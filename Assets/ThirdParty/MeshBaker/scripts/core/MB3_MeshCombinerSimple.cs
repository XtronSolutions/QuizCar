using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Text;
using DigitalOpus.MB.Core;

namespace DigitalOpus.MB.Core{
	/// <summary>
	/// Manages a single combined mesh.This class is the core of the mesh combining API.
	/// 
	/// It is not a component so it can be can be instantiated and used like a normal c sharp class.
	/// </summary>
	[System.Serializable]
	public class MB3_MeshCombinerSingle:MB3_MeshCombiner{		
		/*
		 Stores information about one source game object that has been added to
		 the combined mesh.  
		*/
		[System.Serializable]
		public class MB_DynamicGameObject:IComparable<MB_DynamicGameObject>{
			public int instanceID;
			public string name;
			public int vertIdx;
			public int numVerts;

            //distinct list of bones in the bones array
            public int[] indexesOfBonesUsed = new int[0];
            
            //public Transform[] _originalBones;    //used only for integrity checking
            //public Matrix4x4[] _originalBindPoses; //used only for integrity checking

            public int lightmapIndex=-1;
			public Vector4 lightmapTilingOffset = new Vector4(1f,1f,0f,0f);
					
			public bool show = true;
			
			public bool invertTriangles = false;
			/*
			 combined mesh will have one submesh per result material
			 source meshes can have any number of submeshes. They are mapped to a result submesh based on their material
			 if two different submeshes have the same material they are merged in the same result submesh  
			*/
			
			//These are result mesh submeshCount comine these into a class
			public int[] submeshTriIdxs;
			public int[] submeshNumTris;
			
			//These are source go mesh submeshCount todo combined these into a class
			///maps each submesh in sharedMesh to a target submesh in combined mesh
			public int[] targetSubmeshIdxs;
            
            // the UVRects in the combinedMaterial atlas
            public Rect[] uvRects;

            // the UVRects in the combinedMaterial atlas can be shared by several source materials that do not use the entire rect
            // uvRectMatTilingSubrect is a sub rect inside uvRects for the uvTiling 
            public Rect[] uvSubRectInAtlas;

            // the UVRects that were used for extracting the texture from the source texture
            // considers only mesh uvTiling not material tiling
            public Rect[] uvRectsInSrcFull; 

            // the obUVRect for each submesh
			public Rect[] obUVRects; 		
			public int[][] _submeshTris;
			
			public bool _beingDeleted=false;
			public int  _triangleIdxAdjustment=0;

            //used so we don't have to call GetBones and GetBindposes twice
            public Transform[] _tmpCachedBones;
            public Matrix4x4[] _tmpCachedBindposes;
			
			public int CompareTo(MB_DynamicGameObject b){
				return this.vertIdx - b.vertIdx;
	        }
		}

        //if baking many instances of the same sharedMesh, want to cache these results rather than grab them multiple times from the mesh 
        public class MeshChannels
        {
            public Vector3[] vertices;
            public Vector3[] normals;
            public Vector4[] tangents;
            public Vector2[] uv0raw;
            public Vector2[] uv0modified;
            public Vector2[] uv2;
            public Vector2[] uv3;
            public Vector2[] uv4;
            public Color[] colors;
            public BoneWeight[] boneWeights;
            public Matrix4x4[] bindPoses;
            public int[] triangles;
        }

        public class MeshChannelsCache
        {
            MB3_MeshCombinerSingle mc;
            protected Dictionary<int, MeshChannels> meshID2MeshChannels = new Dictionary<int, MeshChannels>();

            public MeshChannelsCache(MB3_MeshCombinerSingle mcs)
            {
                mc = mcs;
            }

           
            public Vector3[] GetVertices(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(),out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.vertices == null)
                {
                    mc.vertices = m.vertices;
                }
                return mc.vertices;
            }
            public Vector3[] GetNormals(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.normals == null)
                {
                    mc.normals = _getMeshNormals(m);
                }
                return mc.normals;
            }
            public Vector4[] GetTangents(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.tangents == null)
                {
                    mc.tangents = _getMeshTangents(m);
                }
                return mc.tangents;
            }
            
            public Vector2[] GetUv0Raw(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.uv0raw == null)
                {
                    mc.uv0raw = _getMeshUVs(m);
                }
                return mc.uv0raw;
            }
            public Vector2[] GetUv0Modified(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.uv0modified == null)
                {
                    //todo
                    mc.uv0modified = null;
                }
                return mc.uv0modified;
            }
            public Vector2[] GetUv2(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.uv2 == null)
                {
                    mc.uv2 = _getMeshUV2s(m);
                }
                return mc.uv2;
            }
            public Vector2[] GetUv3(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.uv3 == null)
                {
                    mc.uv3 = MBVersion.GetMeshUV3orUV4(m, true, this.mc.LOG_LEVEL);
                }
                return mc.uv3;
            }
            public Vector2[] GetUv4(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.uv4 == null)
                {
                    mc.uv4 = MBVersion.GetMeshUV3orUV4(m, false, this.mc.LOG_LEVEL);
                }
                return mc.uv4;
            }
            public Color[] GetColors(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.colors == null)
                {
                    mc.colors = _getMeshColors(m);
                }
                return mc.colors;
            }
            public Matrix4x4[] GetBindposes(Renderer r)
            {
                MeshChannels mc;
                Mesh m = MB_Utility.GetMesh(r.gameObject);
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.bindPoses == null)
                {
                    mc.bindPoses = _getBindPoses(r);
                }
                return mc.bindPoses;
            }

            public BoneWeight[] GetBoneWeights(Renderer r, int numVertsInMeshBeingAdded)
            {
                MeshChannels mc;
                Mesh m = MB_Utility.GetMesh(r.gameObject);
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.boneWeights == null)
                {
                    mc.boneWeights = _getBoneWeights(r, numVertsInMeshBeingAdded);
                }
                return mc.boneWeights;
            }

            public int[] GetTriangles(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.triangles == null)
                {
                    mc.triangles = m.triangles;
                }
                return mc.triangles;
            }

            Color[] _getMeshColors(Mesh m)
            {
                Color[] cs = m.colors;
                if (cs.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no colors. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have colors. Generating an array of white colors");
                    cs = new Color[m.vertexCount];
                    for (int i = 0; i < cs.Length; i++) { cs[i] = Color.white; }
                }
                return cs;
            }

            Vector3[] _getMeshNormals(Mesh m)
            {
                Vector3[] ns = m.normals;
                if (ns.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no normals. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have normals. Generating normals.");
                    Mesh tempMesh = (Mesh)GameObject.Instantiate(m);
                    tempMesh.RecalculateNormals();
                    ns = tempMesh.normals;
                    MB_Utility.Destroy(tempMesh);
                }
                return ns;
            }

            Vector4[] _getMeshTangents(Mesh m)
            {
                Vector4[] ts = m.tangents;
                if (ts.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no tangents. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have tangents. Generating tangents.");
                    Vector3[] verts = m.vertices;
                    Vector2[] uvs = GetUv0Raw(m);
                    Vector3[] norms = _getMeshNormals(m);
                    ts = new Vector4[m.vertexCount];
                    for (int i = 0; i < m.subMeshCount; i++)
                    {
                        int[] tris = m.GetTriangles(i);
                        _generateTangents(tris, verts, uvs, norms, ts);
                    }
                }
                return ts;
            }

            Vector2 _HALF_UV = new Vector2(.5f, .5f);
            Vector2[] _getMeshUVs(Mesh m)
            {
                Vector2[] uv = m.uv;
                if (uv.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no uvs. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have uvs. Generating uvs.");
                    uv = new Vector2[m.vertexCount];
                    for (int i = 0; i < uv.Length; i++) { uv[i] = _HALF_UV; }
                }
                return uv;
            }

            Vector2[] _getMeshUV2s(Mesh m)
            {
                Vector2[] uv = m.uv2;
                if (uv.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no uv2s. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have uv2s. Generating uv2s.");
                    uv = new Vector2[m.vertexCount];
                    for (int i = 0; i < uv.Length; i++) { uv[i] = _HALF_UV; }
                }
                return uv;
            }

            public static Matrix4x4[] _getBindPoses(Renderer r)
            {
                if (r is SkinnedMeshRenderer)
                {
                    return ((SkinnedMeshRenderer)r).sharedMesh.bindposes;
                }
                else if (r is MeshRenderer)
                {
                    Matrix4x4 bindPose = Matrix4x4.identity;
                    Matrix4x4[] poses = new Matrix4x4[1];
                    poses[0] = bindPose;
                    return poses;
                }
                else {
                    Debug.LogError("Could not _getBindPoses. Object does not have a renderer");
                    return null;
                }
            }

            public static BoneWeight[] _getBoneWeights(Renderer r, int numVertsInMeshBeingAdded)
            {
                if (r is SkinnedMeshRenderer)
                {
                    return ((SkinnedMeshRenderer)r).sharedMesh.boneWeights;
                }
                else if (r is MeshRenderer)
                {
                    BoneWeight bw = new BoneWeight();
                    bw.boneIndex0 = bw.boneIndex1 = bw.boneIndex2 = bw.boneIndex3 = 0;
                    bw.weight0 = 1f;
                    bw.weight1 = bw.weight2 = bw.weight3 = 0f;
                    BoneWeight[] bws = new BoneWeight[numVertsInMeshBeingAdded];
                    for (int i = 0; i < bws.Length; i++) bws[i] = bw;
                    return bws;
                }
                else {
                    Debug.LogError("Could not _getBoneWeights. Object does not have a renderer");
                    return null;
                }
            }


            void _generateTangents(int[] triangles, Vector3[] verts, Vector2[] uvs, Vector3[] normals, Vector4[] outTangents)
            {
                int triangleCount = triangles.Length;
                int vertexCount = verts.Length;

                Vector3[] tan1 = new Vector3[vertexCount];
                Vector3[] tan2 = new Vector3[vertexCount];

                for (int a = 0; a < triangleCount; a += 3)
                {
                    int i1 = triangles[a + 0];
                    int i2 = triangles[a + 1];
                    int i3 = triangles[a + 2];

                    Vector3 v1 = verts[i1];
                    Vector3 v2 = verts[i2];
                    Vector3 v3 = verts[i3];

                    Vector2 w1 = uvs[i1];
                    Vector2 w2 = uvs[i2];
                    Vector2 w3 = uvs[i3];

                    float x1 = v2.x - v1.x;
                    float x2 = v3.x - v1.x;
                    float y1 = v2.y - v1.y;
                    float y2 = v3.y - v1.y;
                    float z1 = v2.z - v1.z;
                    float z2 = v3.z - v1.z;

                    float s1 = w2.x - w1.x;
                    float s2 = w3.x - w1.x;
                    float t1 = w2.y - w1.y;
                    float t2 = w3.y - w1.y;

                    float rBot = (s1 * t2 - s2 * t1);
                    if (rBot == 0f)
                    {
                        Debug.LogError("Could not compute tangents. All UVs need to form a valid triangles in UV space. If any UV triangles are collapsed, tangents cannot be generated.");
                        return;
                    }
                    float r = 1.0f / rBot;

                    Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                    Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                    tan1[i1] += sdir;
                    tan1[i2] += sdir;
                    tan1[i3] += sdir;

                    tan2[i1] += tdir;
                    tan2[i2] += tdir;
                    tan2[i3] += tdir;
                }


                for (int a = 0; a < vertexCount; ++a)
                {
                    Vector3 n = normals[a];
                    Vector3 t = tan1[a];

                    Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
                    outTangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
                    outTangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
                }
            }
        }

        //Used for comparing if skinned meshes use the same bone and bindpose.
        //Skinned meshes must be bound with the same TRS to share a bone.
        public struct BoneAndBindpose {
            public Transform bone;
            public Matrix4x4 bindPose;

            public BoneAndBindpose(Transform t, Matrix4x4 bp) {
                bone = t;
                bindPose = bp;
            }

            public override bool Equals(object obj) {
                if (obj is BoneAndBindpose) {
                    if (bone == ((BoneAndBindpose) obj).bone && bindPose == ((BoneAndBindpose)obj).bindPose){
                        return true;
                    }
                }
                return false;
            }

            public override int GetHashCode() {
                //OK if don't check bindPose well because bp should be the same
                return (bone.GetInstanceID() % 2147483647) ^ (int) bindPose[0,0];
            }
        }

		public override MB2_TextureBakeResults textureBakeResults {  
			set{
				if (objectsInCombinedMesh.Count > 0 && _textureBakeResults != value && _textureBakeResults != null){
					if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("If material bake result is changed then objects currently in combined mesh may be invalid.");	
				}
				_textureBakeResults = value;
			}
		}
		
		public override MB_RenderType renderType { 
			set{
				if (value == MB_RenderType.skinnedMeshRenderer && _renderType == MB_RenderType.meshRenderer){
					if (boneWeights.Length != verts.Length) Debug.LogError("Can't set the render type to SkinnedMeshRenderer without clearing the mesh first. Try deleteing the CombinedMesh scene object.");
				}
				_renderType = value;
			} 
		}
		
		public override GameObject resultSceneObject { 
			set{
				if (_resultSceneObject != value){
					_targetRenderer = null;
					if (_mesh != null && _LOG_LEVEL >= MB2_LogLevel.warn){
						Debug.LogWarning("Result Scene Object was changed when this mesh baker component had a reference to a mesh. If mesh is being used by another object make sure to reset the mesh to none before baking to avoid overwriting the other mesh.");	
					}
				}
				_resultSceneObject = value;
			} 
		}		

		//this contains object instances that have been added to the combined mesh through AddDelete
		[SerializeField] protected List<GameObject> objectsInCombinedMesh = new List<GameObject>();			
		
		[SerializeField] int lightmapIndex = -1;
		
		[SerializeField] List<MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh = new List<MB_DynamicGameObject>();
		Dictionary<int,MB_DynamicGameObject> _instance2combined_map = new Dictionary<int, MB_DynamicGameObject>();
		
		[SerializeField] Vector3[] verts = new Vector3[0];
		[SerializeField] Vector3[] normals = new Vector3[0];
		[SerializeField] Vector4[] tangents = new Vector4[0];
		[SerializeField] Vector2[] uvs = new Vector2[0];
		[SerializeField] Vector2[] uv2s = new Vector2[0];
        [SerializeField] Vector2[] uv3s = new Vector2[0];
        [SerializeField] Vector2[] uv4s = new Vector2[0];
        [SerializeField] Color[] colors = new Color[0];
		[SerializeField] Matrix4x4[] bindPoses = new Matrix4x4[0];
		[SerializeField] Transform[] bones = new Transform[0];
		[SerializeField] Mesh _mesh; 
		
		//unity won't serialize these
		int[][] submeshTris = new int[0][];
		BoneWeight[] boneWeights = new BoneWeight[0];
		
        //used if user passes null in as parameter to AddOrDelete
	    GameObject[] empty = new GameObject[0];
		int[] emptyIDs = new int[0];	
		
		MB_DynamicGameObject instance2Combined_MapGet(int gameObjectID){
			return _instance2combined_map[gameObjectID];
		}

		void instance2Combined_MapAdd(int gameObjectID, MB_DynamicGameObject dgo){
			_instance2combined_map.Add(gameObjectID, dgo);
		}

		void instance2Combined_MapRemove(int gameObjectID){
			_instance2combined_map.Remove(gameObjectID);
		}

		bool instance2Combined_MapTryGetValue(int gameObjectID, out MB_DynamicGameObject dgo){
			return _instance2combined_map.TryGetValue(gameObjectID,out dgo);
		}
		
		int instance2Combined_MapCount(){
			return _instance2combined_map.Count;
		}
		
		void instance2Combined_MapClear(){
			_instance2combined_map.Clear();
		}
		
		bool instance2Combined_MapContainsKey(int gameObjectID){
			return _instance2combined_map.ContainsKey(gameObjectID);	
		}
	
		public override int GetNumObjectsInCombined(){
			return objectsInCombinedMesh.Count;		
		}	
		
		public override List<GameObject> GetObjectsInCombined(){
			List<GameObject> outObs = new List<GameObject>();
			outObs.AddRange(objectsInCombinedMesh);
			return outObs;
		}
		
		public Mesh GetMesh(){
			if (_mesh == null) {
				_mesh = new Mesh();
	//			_mesh.MarkDynamic();			
			}
			return _mesh;	
		}
		
		public Transform[] GetBones(){
			return bones;	
		}
		
		public override int GetLightmapIndex(){
			if (lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout || lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping){
				return lightmapIndex;
			} else {
				return -1;	
			}
		}
		
		public override int GetNumVerticesFor(GameObject go){
			return GetNumVerticesFor(go.GetInstanceID());
		}
		
		public override int GetNumVerticesFor(int instanceID){
			MB_DynamicGameObject dgo;
			if (instance2Combined_MapTryGetValue(instanceID, out dgo)){
				return dgo.numVerts;
			} else {
				return -1;
			}
		}
		
		void _initialize(){	
			if (objectsInCombinedMesh.Count == 0){
				lightmapIndex = -1;
			}
			if (_mesh == null){
				if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("_initialize Creating new Mesh");
				_mesh = GetMesh();
			}
			
			if (instance2Combined_MapCount() != objectsInCombinedMesh.Count){
				//build the instance2Combined map
				instance2Combined_MapClear();
				for (int i = 0; i < objectsInCombinedMesh.Count; i++){
					instance2Combined_MapAdd(objectsInCombinedMesh[i].GetInstanceID(), mbDynamicObjectsInCombinedMesh[i]);
				}
				//BoneWeights are not serialized get from combined mesh
				boneWeights = _mesh.boneWeights;
				//submesh tris are not serialized either get from mesh
				submeshTris = new int[_mesh.subMeshCount][];
				for (int i = 0; i < submeshTris.Length; i++){
					submeshTris[i] = _mesh.GetTriangles(i);	
				}
			}
            //MeshBaker was baked using old system that had duplicated bones. Upgrade to new system
            //need to build indexesOfBonesUsed maps for dgos
            if (mbDynamicObjectsInCombinedMesh.Count > 0 && 
                mbDynamicObjectsInCombinedMesh[0].indexesOfBonesUsed.Length == 0 &&
                renderType == MB_RenderType.skinnedMeshRenderer &&
                boneWeights.Length > 0) {

                for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++) {
                    MB_DynamicGameObject dgo = mbDynamicObjectsInCombinedMesh[i];
                    HashSet<int> idxsOfBonesUsed = new HashSet<int>();
                    for (int j = dgo.vertIdx; j < dgo.vertIdx + dgo.numVerts; j++) {
                        if (boneWeights[j].weight0 > 0f) idxsOfBonesUsed.Add(boneWeights[j].boneIndex0);
                        if (boneWeights[j].weight1 > 0f) idxsOfBonesUsed.Add(boneWeights[j].boneIndex1);
                        if (boneWeights[j].weight2 > 0f) idxsOfBonesUsed.Add(boneWeights[j].boneIndex2);
                        if (boneWeights[j].weight3 > 0f) idxsOfBonesUsed.Add(boneWeights[j].boneIndex3);
                    }
                    dgo.indexesOfBonesUsed = new int[idxsOfBonesUsed.Count];
                    idxsOfBonesUsed.CopyTo(dgo.indexesOfBonesUsed);
                }
                if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Baker used old systems that duplicated bones. Upgrading to new system by building indexesOfBonesUsed");
            }
		}
		
		bool _collectMaterialTriangles(Mesh m, MB_DynamicGameObject dgo,Material[] sharedMaterials, OrderedDictionary sourceMats2submeshIdx_map){
			//everything here applies to the source object being added
			int numTriMeshes = m.subMeshCount;
			if (sharedMaterials.Length < numTriMeshes) numTriMeshes = sharedMaterials.Length;
			dgo._submeshTris = new int[numTriMeshes][];
			dgo.targetSubmeshIdxs = new int[numTriMeshes];
			for(int i = 0; i < numTriMeshes; i++){
				if (textureBakeResults.doMultiMaterial){
					if (!sourceMats2submeshIdx_map.Contains(sharedMaterials[i])){
						Debug.LogError("Object " + dgo.name + " has a material that was not found in the result materials maping. " + sharedMaterials[i]);
						return false;
					}
					dgo.targetSubmeshIdxs[i] = (int) sourceMats2submeshIdx_map[sharedMaterials[i]];
				} else {
					dgo.targetSubmeshIdxs[i] = 0;
				}
				dgo._submeshTris[i] = m.GetTriangles(i);
				if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Collecting triangles for: " + dgo.name + " submesh:" + i + " maps to submesh:" + dgo.targetSubmeshIdxs[i] + " added:" + dgo._submeshTris[i].Length,LOG_LEVEL);
			}
			return true;
		}

		// if adding many copies of the same mesh want to cache obUVsResults
		bool _collectOutOfBoundsUVRects2(Mesh m, MB_DynamicGameObject dgo,Material[] sharedMaterials, OrderedDictionary sourceMats2submeshIdx_map, Dictionary<int,MB_Utility.MeshAnalysisResult[]> meshAnalysisResults, MeshChannelsCache meshChannelCache){
			if (textureBakeResults == null){
				Debug.LogError("Need to bake textures into combined material");	
				return false;
			}
			MB_Utility.MeshAnalysisResult[] res;
			if (meshAnalysisResults.TryGetValue(m.GetInstanceID(),out res)){
				dgo.obUVRects = new Rect[sharedMaterials.Length];
				for(int i = 0; i < dgo.obUVRects.Length; i++){
					dgo.obUVRects[i] = res[i].uvRect;
				}
			} else {
				int numTriMeshes = m.subMeshCount;
				int numUsedTriMeshes = numTriMeshes;
				if (sharedMaterials.Length < numTriMeshes) numUsedTriMeshes = sharedMaterials.Length;
				dgo.obUVRects = new Rect[numUsedTriMeshes];
				//the mesh analysis result might be longer because we are caching and sharing the result with other
				//renderers which may use more materials
				res = new MB_Utility.MeshAnalysisResult[numTriMeshes];
				for(int i = 0; i < numTriMeshes; i++){
                    Vector2[] uvs = meshChannelCache.GetUv0Raw(m);
                    MB_Utility.hasOutOfBoundsUVs(uvs,m,ref res[i],i);
                    Rect r = res[i].uvRect;
					if (i < numUsedTriMeshes) dgo.obUVRects[i] = r;
				}
				meshAnalysisResults.Add (m.GetInstanceID(),res);
			}
			return true;		
		}	
		
		bool _validateTextureBakeResults(){
			if (textureBakeResults == null){
				Debug.LogError("Material Bake Results is null. Can't combine meshes.");	
				return false;
			}
			if ((textureBakeResults.materialsAndUVRects == null || textureBakeResults.materialsAndUVRects.Length == 0) &&
                (textureBakeResults.materials == null || textureBakeResults.materials.Length == 0)){
				Debug.LogError("Material Bake Results has no materials in material to sourceUVRect map. Try baking materials. Can't combine meshes.");	
				return false;			
			}
			if (textureBakeResults.doMultiMaterial){
				if (textureBakeResults.resultMaterials == null || textureBakeResults.resultMaterials.Length == 0){
					Debug.LogError("Material Bake Results has no result materials. Try baking materials. Can't combine meshes.");	
					return false;				
				}
			} else {
				if (textureBakeResults.resultMaterial == null){
					Debug.LogError("Material Bake Results has no result material. Try baking materials. Can't combine meshes.");	
					return false;				
				}
			}
			return true;
		}
		
		bool _validateMeshFlags(){
			if (objectsInCombinedMesh.Count > 0){
				if (_doNorm == false && doNorm == true ||
					_doTan == false && doTan == true ||
					_doCol == false && doCol == true ||
					_doUV == false && doUV == true ||
                    _doUV3 == false && doUV3 == true ||
                    _doUV4 == false && doUV4 == true){
					Debug.LogError("The channels have changed. There are already objects in the combined mesh that were added with a different set of channels.");
					return false;	
				}
			}
			_doNorm = doNorm;
			_doTan = doTan;
			_doCol = doCol;
			_doUV = doUV;
			_doUV3 = doUV3;
            _doUV4 = doUV4;
			return true;
		}
		
		bool _showHide(GameObject[] goToShow, GameObject[] goToHide){
			if (goToShow == null) goToShow = empty;
			if (goToHide == null) goToHide = empty;
			//calculate amount to hide
			_initialize();
			for (int i = 0; i < goToHide.Length; i++){
				if( !instance2Combined_MapContainsKey(goToHide[i].GetInstanceID())){
					if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Trying to hide an object "+goToHide[i]+" that is not in combined mesh. Did you initially bake with 'clear buffers after bake' enabled?");
					return false;
				}
			}
			
			//now to show
			for (int i = 0; i < goToShow.Length; i++){
				if( !instance2Combined_MapContainsKey(goToShow[i].GetInstanceID())){			
					if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Trying to show an object "+goToShow[i]+" that is not in combined mesh. Did you initially bake with 'clear buffers after bake' enabled?");
					return false;
				}
			}
			
			//set flags
			for (int i = 0; i < goToHide.Length; i++) _instance2combined_map[goToHide[i].GetInstanceID()].show = false;
			for (int i = 0; i < goToShow.Length; i++) _instance2combined_map[goToShow[i].GetInstanceID()].show = true;
 
			return true;
		}
		
		bool _addToCombined(GameObject[] goToAdd, int[] goToDelete,bool disableRendererInSource){
			GameObject[] _goToAdd; 
			int[] _goToDelete;
			if (!_validateTextureBakeResults()) return false;
			if (!_validateMeshFlags()) return false;
			if (!ValidateTargRendererAndMeshAndResultSceneObj()) return false;
			
			if (outputOption != MB2_OutputOptions.bakeMeshAssetsInPlace && 
				renderType == MB_RenderType.skinnedMeshRenderer){
				if (_targetRenderer == null || !(_targetRenderer is SkinnedMeshRenderer)){
					Debug.LogError("Target renderer must be set and must be a SkinnedMeshRenderer");
					return false;
				}
				SkinnedMeshRenderer smr = (SkinnedMeshRenderer) targetRenderer;
				if (smr.sharedMesh != _mesh){
					Debug.LogError ("The combined mesh was not assigned to the targetRenderer. Try using buildSceneMeshObject to set up the combined mesh correctly");
				}
			}
			if (goToAdd == null) _goToAdd = empty;
			else _goToAdd = (GameObject[]) goToAdd.Clone();
			if (goToDelete == null) _goToDelete = emptyIDs;
			else _goToDelete = (int[]) goToDelete.Clone();
			if (_mesh == null) DestroyMesh(); //cleanup maps and arrays

			MB2_TextureBakeResults.Material2AtlasRectangleMapper mat2rect_map = new MB2_TextureBakeResults.Material2AtlasRectangleMapper(textureBakeResults);
			
			_initialize();

			if (_mesh.vertexCount > 0 && _instance2combined_map.Count == 0) {
				Debug.LogWarning("There were vertices in the combined mesh but nothing in the MeshBaker buffers. If you are trying to bake in the editor and modify at runtime, make sure 'Clear Buffers After Bake' is unchecked.");
			}
			
			int numResultMats = 1;
			if (textureBakeResults.doMultiMaterial) numResultMats = textureBakeResults.resultMaterials.Length;
			
			if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("==== Calling _addToCombined objs adding:" + _goToAdd.Length + " objs deleting:" + _goToDelete.Length + " fixOutOfBounds:" + textureBakeResults.fixOutOfBoundsUVs + " doMultiMaterial:" + textureBakeResults.doMultiMaterial + " disableRenderersInSource:" + disableRendererInSource,LOG_LEVEL);
			
			OrderedDictionary  sourceMats2submeshIdx_map = null;
			if (textureBakeResults.doMultiMaterial){
				//build the sourceMats to submesh index map
				sourceMats2submeshIdx_map = new OrderedDictionary ();
				for(int i = 0; i < numResultMats; i++){
					MB_MultiMaterial mm = textureBakeResults.resultMaterials[i];				
					for (int j = 0; j < mm.sourceMaterials.Count; j++){
						if (mm.sourceMaterials[j] == null){
							Debug.LogError("Found null material in source materials for combined mesh materials " + i);
							return false;
						}
						if (!sourceMats2submeshIdx_map.Contains(mm.sourceMaterials[j])){
							sourceMats2submeshIdx_map.Add(mm.sourceMaterials[j],i);
						}
					}
				}
			}
			
			if (submeshTris.Length != numResultMats){
				submeshTris = new int[numResultMats][];
				for (int i = 0; i < submeshTris.Length;i++) submeshTris[i] = new int[0];
			}
			
			//STEP 1 update our internal description of objects being added and deleted keep track of changes to buffer sizes as we do.
			//calculate amount to delete
			int totalDeleteVerts = 0;
			int[] totalDeleteSubmeshTris = new int[numResultMats];

            //in order to decide if a bone can be deleted need to know which objects use it so build a map
            List<MB_DynamicGameObject>[] boneIdx2dgoMap = null;
            List<int> boneIdxsToDelete = new List<int>();
            HashSet<BoneAndBindpose> bonesToAdd = new HashSet<BoneAndBindpose>();
            if (renderType == MB_RenderType.skinnedMeshRenderer && _goToDelete.Length > 0) {
                boneIdx2dgoMap = _buildBoneIdx2dgoMap();
            }
			for (int i = 0; i < _goToDelete.Length; i++){
				MB_DynamicGameObject dgo;
				if( instance2Combined_MapTryGetValue(_goToDelete[i],out dgo)){
					totalDeleteVerts += dgo.numVerts;
					if (renderType == MB_RenderType.skinnedMeshRenderer){
                        for (int j = 0; j < dgo.indexesOfBonesUsed.Length; j++) {
                            if (boneIdx2dgoMap[dgo.indexesOfBonesUsed[j]].Contains(dgo)) {
                                boneIdx2dgoMap[dgo.indexesOfBonesUsed[j]].Remove(dgo);
                                if (boneIdx2dgoMap[dgo.indexesOfBonesUsed[j]].Count == 0) {
                                    boneIdxsToDelete.Add(dgo.indexesOfBonesUsed[j]);
                                }
                            }
                        }
					}
					for (int j = 0; j < dgo.submeshNumTris.Length; j++){
						totalDeleteSubmeshTris[j] += dgo.submeshNumTris[j];	
					}
				}else{
					if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Trying to delete an object that is not in combined mesh");	
				}
			}
			
			//now add
			List<MB_DynamicGameObject> toAddDGOs = new List<MB_DynamicGameObject>();
			Dictionary<int,MB_Utility.MeshAnalysisResult[]> meshAnalysisResultsCache = new Dictionary<int, MB_Utility.MeshAnalysisResult[]>(); //cache results

            //we are often adding the same sharedMesh many times. Only want to grab the results once and cache them
            MeshChannelsCache meshChannelCache = new MeshChannelsCache(this); 

			int totalAddVerts = 0;
			int[] totalAddSubmeshTris = new int[numResultMats];
			for (int i = 0; i < _goToAdd.Length; i++){
				// if not already in mesh or we are deleting and re-adding in same operation
				if(!instance2Combined_MapContainsKey(_goToAdd[i].GetInstanceID()) || Array.FindIndex<int>(_goToDelete,o => o == _goToAdd[i].GetInstanceID()) != -1){
					MB_DynamicGameObject dgo = new MB_DynamicGameObject();
	
					GameObject go = _goToAdd[i];
					
					Material[] sharedMaterials = MB_Utility.GetGOMaterials(go);
	
					if (sharedMaterials == null){
						Debug.LogError("Object " + go.name + " does not have a Renderer");
						_goToAdd[i] = null;
						return false;
					}
	
					Mesh m = MB_Utility.GetMesh(go);				
					if (m == null){
						Debug.LogError("Object " + go.name + " MeshFilter or SkinedMeshRenderer had no mesh");
						_goToAdd[i] = null;	
						return false;
					} else if (MBVersion.IsRunningAndMeshNotReadWriteable(m)){
						Debug.LogError("Object " + go.name + " Mesh Importer has read/write flag set to 'false'. This needs to be set to 'true' in order to read data from this mesh.");
						_goToAdd[i] = null;	
						return false;
					}
					
					Rect[] uvRectsInAtlas = new Rect[sharedMaterials.Length];
                    Rect[] uvRectsInSrcFull = new Rect[sharedMaterials.Length];
                    Rect[] uvSubRectsInAtlas = new Rect[sharedMaterials.Length];
                    String errorMsg = "";
                    for (int j = 0; j < sharedMaterials.Length; j++){
                        if (!mat2rect_map.TryMapMaterialToUVRect(sharedMaterials[j], m, j, meshChannelCache, meshAnalysisResultsCache, out uvRectsInAtlas[j], out uvSubRectsInAtlas[j], ref errorMsg)){
							Debug.LogError(errorMsg);
							_goToAdd[i] = null;			
						}
					}				
					if (_goToAdd[i] != null){
						toAddDGOs.Add(dgo);
						dgo.name = String.Format("{0} {1}",_goToAdd[i].ToString(), _goToAdd[i].GetInstanceID());
						dgo.instanceID = _goToAdd[i].GetInstanceID();
						dgo.uvRects = uvRectsInAtlas;
                        dgo.uvRectsInSrcFull = uvRectsInSrcFull;
                        dgo.uvSubRectInAtlas = uvSubRectsInAtlas;
						dgo.numVerts = m.vertexCount;
						Renderer r = MB_Utility.GetRenderer(go);
						if (renderType == MB_RenderType.skinnedMeshRenderer) {
                            _CollectBonesToAddForDGO(dgo, boneIdxsToDelete, bonesToAdd, r, meshChannelCache);
                        }
						if (lightmapIndex == -1) {
							lightmapIndex = r.lightmapIndex; //initialize	
						}
						if (lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping){
							if (lightmapIndex != r.lightmapIndex){
								if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Object " + go.name + " has a different lightmap index. Lightmapping will not work.");						
							}
							if (!MBVersion.GetActive(go)){
								if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Object " + go.name + " is inactive. Can only get lightmap index of active objects.");													
							}
							if (r.lightmapIndex == -1){
								if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Object " + go.name + " does not have an index to a lightmap.");													
							}						
						}
						dgo.lightmapIndex = r.lightmapIndex;
						dgo.lightmapTilingOffset = MBVersion.GetLightmapTilingOffset(r);
						if (!_collectMaterialTriangles(m,dgo,sharedMaterials,sourceMats2submeshIdx_map)){
							return false;
						}
						dgo.submeshNumTris = new int[numResultMats];
						dgo.submeshTriIdxs = new int[numResultMats];
	
						if (textureBakeResults.fixOutOfBoundsUVs){
							if (!_collectOutOfBoundsUVRects2(m,dgo,sharedMaterials,sourceMats2submeshIdx_map,meshAnalysisResultsCache,meshChannelCache)){
								return false;
							}
						}
						totalAddVerts += dgo.numVerts;
						
						for (int j = 0; j < dgo._submeshTris.Length; j++){
							totalAddSubmeshTris[dgo.targetSubmeshIdxs[j]] += dgo._submeshTris[j].Length;
						}
						
						dgo.invertTriangles = IsMirrored(go.transform.localToWorldMatrix);
					}
				}else{
					if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Object " + _goToAdd[i].name + " has already been added");
					_goToAdd[i] = null;
				}
			}
			
			for (int i = 0; i < _goToAdd.Length; i++){
				if (_goToAdd[i] != null && disableRendererInSource){ 
					MB_Utility.DisableRendererInSource(_goToAdd[i]);
					if (LOG_LEVEL == MB2_LogLevel.trace) Debug.Log("Disabling renderer on " + _goToAdd[i].name + " id=" + _goToAdd[i].GetInstanceID());
				}
			}
			
			//STEP 2 to allocate new buffers and copy everything over
			int newVertSize = verts.Length + totalAddVerts - totalDeleteVerts;
			int newBonesSize = bindPoses.Length + bonesToAdd.Count - boneIdxsToDelete.Count;
			int[] newSubmeshTrisSize = new int[numResultMats];
			if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Verts adding:" +totalAddVerts + " deleting:" + totalDeleteVerts + " submeshes:" + newSubmeshTrisSize.Length + " bones:" + newBonesSize,LOG_LEVEL);
	
			for (int i = 0; i < newSubmeshTrisSize.Length; i++){
				newSubmeshTrisSize[i] = submeshTris[i].Length + totalAddSubmeshTris[i] - totalDeleteSubmeshTris[i];	
				if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug( "    submesh :" + i + " already contains:" + submeshTris[i].Length + " tris to be Added:" + totalAddSubmeshTris[i] + " tris to be Deleted:" + totalDeleteSubmeshTris[i]);
			}		
			
			if (newVertSize > 65534){
				Debug.LogError("Cannot add objects. Resulting mesh will have more than 64k vertices. Try using a Multi-MeshBaker component. This will split the combined mesh into several meshes. You don't have to re-configure the MB2_TextureBaker. Just remove the MB2_MeshBaker component and add a MB2_MultiMeshBaker component.");
				return false;				
			}		
			
			Vector3[] nnormals = null;
			Vector4[] ntangents = null;
			Vector2[] nuvs = null, nuv2s = null, nuv3s = null, nuv4s = null;
			Color[] ncolors = null;
			
			Vector3[] nverts = new Vector3[newVertSize];
			
			if (_doNorm) nnormals = new Vector3[newVertSize];
			if (_doTan) ntangents = new Vector4[newVertSize];
			if (_doUV) nuvs = new Vector2[newVertSize];
			if (_doUV3) nuv3s = new Vector2[newVertSize];
            if (_doUV4) nuv4s = new Vector2[newVertSize];
            if (lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged ||
                lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping) {
                nuv2s = new Vector2[newVertSize];
            }
			if (_doCol) ncolors = new Color[newVertSize];
			
			BoneWeight[] nboneWeights = new BoneWeight[newVertSize];
			Matrix4x4[] nbindPoses = new Matrix4x4[newBonesSize];
			Transform[] nbones = new Transform[newBonesSize];
			int[][] nsubmeshTris = null;
			
			nsubmeshTris = new int[numResultMats][];
			for (int i = 0; i < nsubmeshTris.Length; i++){
				nsubmeshTris[i] = new int[newSubmeshTrisSize[i]];
			}
			
			for (int i = 0; i < _goToDelete.Length; i++){
				MB_DynamicGameObject dgo = null; 
				if (instance2Combined_MapTryGetValue(_goToDelete[i], out dgo)){
					dgo._beingDeleted = true;
				}
			}		
			
			mbDynamicObjectsInCombinedMesh.Sort();
			
			//copy existing arrays to narrays gameobj by gameobj omitting deleted ones
			int targVidx = 0;
			//int targBidx = 0;
			int[] targSubmeshTidx = new int[numResultMats];
			int triangleIdxAdjustment = 0;
			//int boneIdxAdjustment = 0;
			for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++){
				MB_DynamicGameObject dgo = mbDynamicObjectsInCombinedMesh[i];
                if (!dgo._beingDeleted) {
                    if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Copying obj in combined arrays idx:" + i, LOG_LEVEL);
                    Array.Copy(verts, dgo.vertIdx, nverts, targVidx, dgo.numVerts);
                    if (_doNorm) { Array.Copy(normals, dgo.vertIdx, nnormals, targVidx, dgo.numVerts); }
                    if (_doTan) { Array.Copy(tangents, dgo.vertIdx, ntangents, targVidx, dgo.numVerts); }
                    if (_doUV) { Array.Copy(uvs, dgo.vertIdx, nuvs, targVidx, dgo.numVerts); }
                    if (_doUV3) { Array.Copy(uv3s, dgo.vertIdx, nuv3s, targVidx, dgo.numVerts); }
                    if (_doUV4) { Array.Copy(uv4s, dgo.vertIdx, nuv4s, targVidx, dgo.numVerts); }
                    if (doUV2()) { Array.Copy(uv2s, dgo.vertIdx, nuv2s, targVidx, dgo.numVerts); }
                    if (_doCol) { Array.Copy(colors, dgo.vertIdx, ncolors, targVidx, dgo.numVerts); }
                    if (renderType == MB_RenderType.skinnedMeshRenderer) { Array.Copy(boneWeights, dgo.vertIdx, nboneWeights, targVidx, dgo.numVerts);}

                    //adjust triangles, then copy them over
                    for (int subIdx = 0; subIdx < numResultMats; subIdx++){
						int[] sTris = submeshTris[subIdx];
						int sTriIdx = dgo.submeshTriIdxs[subIdx];
						int sNumTris = dgo.submeshNumTris[subIdx];
						if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("    Adjusting submesh triangles submesh:"+subIdx+" startIdx:"+sTriIdx+" num:"+sNumTris,LOG_LEVEL);
						for (int j = sTriIdx; j < sTriIdx + sNumTris; j++){
							sTris[j] = sTris[j] - triangleIdxAdjustment;
						}								
						Array.Copy(sTris,sTriIdx,nsubmeshTris[subIdx],targSubmeshTidx[subIdx],sNumTris);
					}
					
					dgo.vertIdx = targVidx;
	
					for (int j = 0; j < targSubmeshTidx.Length; j++){
						dgo.submeshTriIdxs[j] = targSubmeshTidx[j];
						targSubmeshTidx[j] += dgo.submeshNumTris[j];
					}				
					targVidx += dgo.numVerts;
				} else {
					if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Not copying obj: " + i,LOG_LEVEL);
					triangleIdxAdjustment += dgo.numVerts;
					//boneIdxAdjustment += dgo.numBones;
				}
			}
            if (renderType == MB_RenderType.skinnedMeshRenderer) {
                _CopyBonesWeAreKeepingToNewBonesArrayAndAdjustBWIndexes(boneIdxsToDelete,bonesToAdd,nbones,nbindPoses,nboneWeights,totalDeleteVerts);
            }
			
			//remove objects we are deleting
			for (int i = mbDynamicObjectsInCombinedMesh.Count - 1; i >= 0;i--){
				if (mbDynamicObjectsInCombinedMesh[i]._beingDeleted){
					instance2Combined_MapRemove(mbDynamicObjectsInCombinedMesh[i].instanceID);
					objectsInCombinedMesh.RemoveAt(i);
					mbDynamicObjectsInCombinedMesh.RemoveAt(i);	
				}
			}
	
			verts = nverts;
			if (_doNorm) normals = nnormals;
			if (_doTan) tangents = ntangents;
			if (_doUV) uvs = nuvs;
			if (_doUV3) uv3s = nuv3s;
            if (_doUV4) uv4s = nuv4s;
            if (doUV2()) uv2s = nuv2s;
			if (_doCol) colors = ncolors;
			if (renderType == MB_RenderType.skinnedMeshRenderer) boneWeights = nboneWeights;
            int newBonesStartAtIdx = bones.Length - boneIdxsToDelete.Count;
            bindPoses = nbindPoses;
            bones = nbones;
            submeshTris = nsubmeshTris;

            
            //insert the new bones into the bones array
            int bidx = 0;
            foreach (BoneAndBindpose t in bonesToAdd) {
                nbones[newBonesStartAtIdx + bidx] = t.bone;
                nbindPoses[newBonesStartAtIdx + bidx] = t.bindPose;
                bidx++;
            }

			//add new
			for (int i = 0; i < toAddDGOs.Count; i++){
				MB_DynamicGameObject dgo = toAddDGOs[i];
				GameObject go = _goToAdd[i];
				int vertsIdx = targVidx;
//				Profile.StartProfile("TestNewNorm");
				Mesh mesh = MB_Utility.GetMesh(go);
				Matrix4x4 l2wMat = go.transform.localToWorldMatrix;
				
				//same as l2w with translation removed
				Matrix4x4 l2wRotScale = l2wMat;
				l2wRotScale[0,3] = l2wRotScale[1,3] = l2wRotScale[2,3] = 0f;

                //can't modify the arrays we get from the cache because they will be modified again
				nverts = meshChannelCache.GetVertices(mesh); 
				Vector3[] nnorms = null; 
				Vector4[] ntangs = null;
				if (_doNorm) nnorms = meshChannelCache.GetNormals(mesh);
				if (_doTan) ntangs = meshChannelCache.GetTangents(mesh);
				if (renderType != MB_RenderType.skinnedMeshRenderer){ //for skinned meshes leave in bind pose
					for (int j = 0; j < nverts.Length; j++){
                        int vIdx = vertsIdx + j;
						verts[vertsIdx + j] = l2wMat.MultiplyPoint3x4(nverts[j]);
						if (_doNorm) {
                            normals[vIdx] = l2wRotScale.MultiplyPoint3x4(nnorms[j]);
                            normals[vIdx] = normals[vIdx].normalized;
						}
						if (_doTan){
							float w = ntangs[j].w; //need to preserve the w value
							Vector3 tn = l2wRotScale.MultiplyPoint3x4(ntangs[j]);
							tn.Normalize();
                            tangents[vIdx] = tn;
							tangents[vIdx].w = w;
						}
					}
				} else {
					if (_doNorm) nnorms.CopyTo(normals,vertsIdx);
					if (_doTan) ntangs.CopyTo(tangents,vertsIdx);							
					nverts.CopyTo(verts,vertsIdx);				
				}
//				Profile.EndProfile("TestNewNorm");
				
				int numTriSets = mesh.subMeshCount;
				if (dgo.uvRects.Length < numTriSets){
					if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + dgo.name + " has more submeshes than materials");
					numTriSets = dgo.uvRects.Length;
				} else if (dgo.uvRects.Length > numTriSets){
					if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + dgo.name + " has fewer submeshes than materials");
				}
								
				if (_doUV){
					_copyAndAdjustUVsFromMesh(dgo, mesh,vertsIdx,meshChannelCache);
				}
				
				if (doUV2()){
					_copyAndAdjustUV2FromMesh(dgo, mesh,vertsIdx, meshChannelCache);
				}
				
				if (_doUV3){
                    nuv3s = meshChannelCache.GetUv3(mesh);
					nuv3s.CopyTo(uv3s,vertsIdx);
				}

                if (_doUV4) {
                    nuv4s = meshChannelCache.GetUv4(mesh); //MBVersion.GetMeshUV3orUV4(mesh,false,LOG_LEVEL);
                    nuv4s.CopyTo(uv4s, vertsIdx);
                }

                if (_doCol){
					ncolors = meshChannelCache.GetColors(mesh);
					ncolors.CopyTo(colors,vertsIdx);
				}
				
				if (renderType == MB_RenderType.skinnedMeshRenderer){
                    Renderer r = MB_Utility.GetRenderer(go);
                    _AddBonesToNewBonesArrayAndAdjustBWIndexes(dgo, r, vertsIdx, nbones, nboneWeights, meshChannelCache);
                }						
				
				for (int combinedMeshIdx = 0; combinedMeshIdx < targSubmeshTidx.Length; combinedMeshIdx++){
					dgo.submeshTriIdxs[combinedMeshIdx] = targSubmeshTidx[combinedMeshIdx];
				}
				for (int j = 0; j < dgo._submeshTris.Length; j++){
					int[] sts = dgo._submeshTris[j];
					for (int k = 0; k < sts.Length; k++){
						sts[k] = sts[k] + vertsIdx;
					}
					if (dgo.invertTriangles){
						//need to reverse winding order
						for (int k = 0; k < sts.Length; k+=3){
							int tmp = sts[k];
							sts[k] = sts[k+1];
							sts[k+1] = tmp;
						}
					}
					int combinedMeshIdx = dgo.targetSubmeshIdxs[j];
					sts.CopyTo(submeshTris[combinedMeshIdx], targSubmeshTidx[combinedMeshIdx]);
					dgo.submeshNumTris[combinedMeshIdx] += sts.Length;
					targSubmeshTidx[combinedMeshIdx] += sts.Length;
				}
							
				dgo.vertIdx = targVidx;
				
				instance2Combined_MapAdd(go.GetInstanceID(),dgo);
				objectsInCombinedMesh.Add(go);
				mbDynamicObjectsInCombinedMesh.Add(dgo);
	
				targVidx += nverts.Length;
				for (int j = 0; j < dgo._submeshTris.Length; j++) dgo._submeshTris[j] = null;
				dgo._submeshTris = null;
				if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Added to combined:" + dgo.name + " verts:" + nverts.Length + " bindPoses:" + nbindPoses.Length,LOG_LEVEL);
			}
			if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("===== _addToCombined completed. Verts in buffer: " + verts.Length,LOG_LEVEL);
			return true;
		}
		
		void _copyAndAdjustUVsFromMesh(MB_DynamicGameObject dgo, Mesh mesh, int vertsIdx, MeshChannelsCache meshChannelsCache)
        {
			Vector2[] nuvs = meshChannelsCache.GetUv0Raw(mesh);
			bool needToModfyUVs = true;
			if (textureBakeResults.fixOutOfBoundsUVs == false){
				Rect ident = new Rect(0f,0f,1f,1f);
				bool allAreIdent = true;
				for (int i = 0 ; i < textureBakeResults.materialsAndUVRects.Length; i++){
					if (textureBakeResults.materialsAndUVRects[i].atlasRect != ident){
						allAreIdent = false;
						break;
					}
				}
				if (allAreIdent) {
					needToModfyUVs = false;
					if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log ("All atlases have only one texture in atlas UVs will be copied without adjusting");
				}
			}			
			if (needToModfyUVs) {
					int[] done = new int[nuvs.Length]; //use this to track uvs that have already been adjusted don't adjust twice
					for (int l = 0; l < done.Length; l++)
							done [l] = -1;
					bool triangleArraysOverlap = false;
					//Rect uvRectInSrc = new Rect (0f,0f,1f,1f);
					//need to address the UVs through the submesh indexes because
					//each submesh has a different UV index
					for (int k = 0; k < dgo.targetSubmeshIdxs.Length; k++) {
							int[] subTris; 
							if (dgo._submeshTris != null)
									subTris = dgo._submeshTris [k];
							else
									subTris = mesh.GetTriangles (k);
							DRect uvRectAtlas = new DRect(dgo.uvRects [k]); //this is uvRect in combined Atlas
                            DRect obUVRect;
                            if (textureBakeResults.fixOutOfBoundsUVs)
                            {
                                obUVRect = new DRect(dgo.obUVRects[k]); //this is the uvRect in src that was used to sample from source to combined atlas
                            } else
                            {
                                obUVRect = new DRect(0.0, 0.0, 1.0, 1.0);
                            }
                            DRect matUVRectSrc = new DRect(dgo.uvSubRectInAtlas[k]);
                            //this is the offset and scale in uvRectAtlas due to material tiling only
                            //if (textureBakeResults.fixOutOfBoundsUVs)
                            //    uvRectSrc = dgo.uvRectsInSrcFull [k]; //this is the uvRect of the mesh we are adding that encapsulates its UVs
                            //                                      //scale and shift the uvRectSrc to accomodate material tiling


                            DRect invMeshUVRect = MB3_UVTransformUtility.InverseTransform(ref obUVRect);                           
                            DRect rAdjInAtlas = MB3_UVTransformUtility.CombineTransforms(ref invMeshUVRect, ref matUVRectSrc);
                            rAdjInAtlas = MB3_UVTransformUtility.CombineTransforms(ref rAdjInAtlas, ref obUVRect);
                            rAdjInAtlas = MB3_UVTransformUtility.CombineTransforms(ref rAdjInAtlas, ref uvRectAtlas); 
                            Rect rr = rAdjInAtlas.GetRect();        

                            //Rect rAdjInAtlas = MB3_UVTransformUtility.CombineTransforms(ref matUVRectSrc, ref uvRectAtlas);
                            for (int l = 0; l < subTris.Length; l++) {
									int vidx = subTris [l];
									if (done [vidx] == -1) {
											done [vidx] = k; //prevents a uv from being adjusted twice. Same vert can be on more than one submesh.
											Vector2 nuv = nuvs [vidx]; //don't modify nuvs directly because it is cached and we might be re-using
                                                                       //if (textureBakeResults.fixOutOfBoundsUVs) {
                                                                       //uvRectInSrc can be larger than (out of bounds uvs) or smaller than 0..1
                                                                       //this transforms the uvs so they fit inside the uvRectInSrc sample box 


                                            // scale, shift to fit in atlas rect
                                            nuv.x = rr.x + nuv.x * rr.width;
											nuv.y = rr.y + nuv.y * rr.height;
											uvs [vertsIdx + vidx] = nuv;
									}
									if (done [vidx] != k) {
											triangleArraysOverlap = true;	
									}
							}
					}
					if (triangleArraysOverlap) {
							if (LOG_LEVEL >= MB2_LogLevel.warn)
									Debug.LogWarning (dgo.name + "has submeshes which share verticies. Adjusted uvs may not map correctly in combined atlas.");	
					}
			} else {
				nuvs.CopyTo(uvs,vertsIdx);
			}
			if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log(string.Format("_copyAndAdjustUVsFromMesh copied {0} verts", nuvs.Length));
		}
		
		void _copyAndAdjustUV2FromMesh(MB_DynamicGameObject dgo, Mesh mesh, int vertsIdx, MeshChannelsCache meshChannelsCache)
        {
			Vector2[] nuv2s = meshChannelsCache.GetUv2(mesh);			
			if (lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping){ //has a lightmap
                //this does not work in Unity 5. the lightmapTilingOffset is always 1,1,0,0 for all objects
                //lightMap index is always 1
                Vector2 uvscale2;
				Vector4 lightmapTilingOffset = dgo.lightmapTilingOffset;
				Vector2 uvscale = new Vector2( lightmapTilingOffset.x, lightmapTilingOffset.y );	
				Vector2 uvoffset = new Vector2( lightmapTilingOffset.z, lightmapTilingOffset.w );
				for ( int j = 0; j < nuv2s.Length; j++ ) {
					uvscale2.x = uvscale.x * nuv2s[j].x;
					uvscale2.y = uvscale.y * nuv2s[j].y;
					uv2s[vertsIdx + j] = uvoffset + uvscale2;
				}
				if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log("_copyAndAdjustUV2FromMesh copied and modify for preserve current lightmapping " + nuv2s.Length);
			} else if (LOG_LEVEL >= MB2_LogLevel.trace){
				nuv2s.CopyTo (uv2s, vertsIdx);
				Debug.Log("_copyAndAdjustUV2FromMesh copied without modifying " + nuv2s.Length);
			}
		}	
		
		public override void UpdateSkinnedMeshApproximateBounds(){
			UpdateSkinnedMeshApproximateBoundsFromBounds();
		}
		
		public override void UpdateSkinnedMeshApproximateBoundsFromBones(){
			if (outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace){
				if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBounds when output type is bakeMeshAssetsInPlace");
				return;
			}
			if (bones.Length == 0){
				if (verts.Length > 0) if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("No bones in SkinnedMeshRenderer. Could not UpdateSkinnedMeshApproximateBounds.");
				return;
			}
			if (_targetRenderer == null){
				if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Target Renderer is not set. No point in calling UpdateSkinnedMeshApproximateBounds.");
				return;			
			}
			if (!_targetRenderer.GetType().Equals( typeof(SkinnedMeshRenderer) )){
				if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Target Renderer is not a SkinnedMeshRenderer. No point in calling UpdateSkinnedMeshApproximateBounds.");
				return;			
			}
			UpdateSkinnedMeshApproximateBoundsFromBonesStatic(bones, (SkinnedMeshRenderer) targetRenderer);
		}
		
		public override void UpdateSkinnedMeshApproximateBoundsFromBounds(){
			if (outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace){
				if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBoundsFromBounds when output type is bakeMeshAssetsInPlace");
				return;
			}
			if (verts.Length == 0 || objectsInCombinedMesh.Count == 0){
				if (verts.Length > 0) if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Nothing in SkinnedMeshRenderer. Could not UpdateSkinnedMeshApproximateBoundsFromBounds.");
				return;
			}
			if (_targetRenderer == null){
				if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Target Renderer is not set. No point in calling UpdateSkinnedMeshApproximateBoundsFromBounds.");
				return;			
			}
			if (!_targetRenderer.GetType().Equals( typeof(SkinnedMeshRenderer) )){
				if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Target Renderer is not a SkinnedMeshRenderer. No point in calling UpdateSkinnedMeshApproximateBoundsFromBounds.");
				return;			
			}
			
			UpdateSkinnedMeshApproximateBoundsFromBoundsStatic(objectsInCombinedMesh,(SkinnedMeshRenderer)targetRenderer);
		}
		
		int _getNumBones(Renderer r){
			if (renderType == MB_RenderType.skinnedMeshRenderer){
				if (r is SkinnedMeshRenderer){
					return ((SkinnedMeshRenderer)r).bones.Length;	
				} else if (r is MeshRenderer){
					return 1;
				} else {
					Debug.LogError("Could not _getNumBones. Object does not have a renderer");
					return 0;
				}
			} else {
				return 0;	
			}
		}
		
		Transform[] _getBones(Renderer r){
			return MBVersion.GetBones(r);
		}
		
		public override void Apply(GenerateUV2Delegate uv2GenerationMethod){
			bool doBones = false;
			if (renderType == MB_RenderType.skinnedMeshRenderer) doBones = true;
			Apply(true,true,_doNorm,_doTan,_doUV,doUV2(),_doUV3,_doUV4,doCol,doBones, uv2GenerationMethod);
		}
		
		public virtual void ApplyShowHide(){
			if (_validationLevel >= MB2_ValidationLevel.quick && !ValidateTargRendererAndMeshAndResultSceneObj()) return;
			if (_mesh != null){
				if (renderType == MB_RenderType.meshRenderer){
					//for MeshRenderer meshes this is needed for adding. It breaks skinnedMeshRenderers
					MBVersion.MeshClear(_mesh,true);
					_mesh.vertices = verts;
				}
				int[][] submeshTrisToUse = GetSubmeshTrisWithShowHideApplied();
				if (textureBakeResults.doMultiMaterial){
					_mesh.subMeshCount = submeshTrisToUse.Length;
					for (int i = 0; i < submeshTrisToUse.Length; i++){
						_mesh.SetTriangles(submeshTrisToUse[i],i);
					}
				} else {
					_mesh.triangles = submeshTrisToUse[0];
				}
				if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log("ApplyShowHide");
			} else {
				Debug.LogError("Need to add objects to this meshbaker before calling ApplyShowHide");	
			}
		}
		

		public override void Apply(bool triangles,
						  bool vertices,
						  bool normals,
						  bool tangents,
						  bool uvs,
                          bool uv2,
                          bool uv3,
                          bool uv4,
						  bool colors,
						  bool bones=false,
						  GenerateUV2Delegate uv2GenerationMethod = null){
			if (_validationLevel >= MB2_ValidationLevel.quick && !ValidateTargRendererAndMeshAndResultSceneObj()) return;
			if (_mesh != null){
				if (LOG_LEVEL >= MB2_LogLevel.trace){
					Debug.Log(String.Format("Apply called tri={0} vert={1} norm={2} tan={3} uv={4} col={5} uv3={6} uv4={7} uv2={8} bone={9} meshID={10}",
						triangles, vertices,normals,tangents,uvs,colors,uv3,uv4,uv2,bones,_mesh.GetInstanceID()));	
				}
				if (triangles || _mesh.vertexCount != verts.Length){
					if (triangles && !vertices &&  !normals &&  !tangents && !uvs &&  !colors &&  !uv3 && !uv4 && !uv2 && !bones){
						MBVersion.MeshClear(_mesh,true); //clear just triangles 
					} else {//clear all the data and start with a blank mesh
						MBVersion.MeshClear(_mesh,false);
					}
				}
				if (vertices)  _mesh.vertices = verts;
				if (triangles && _textureBakeResults){
					if (_textureBakeResults == null){
						Debug.LogError("Material Bake Result was not set.");
					} else {
						int[][] submeshTrisToUse = GetSubmeshTrisWithShowHideApplied();
						if (_textureBakeResults.doMultiMaterial){
							_mesh.subMeshCount = submeshTrisToUse.Length;
							for (int i = 0; i < submeshTrisToUse.Length; i++){
								_mesh.SetTriangles(submeshTrisToUse[i],i);
							}
						} else {
							_mesh.triangles = submeshTrisToUse[0];
						}
					}
				}
				if (normals){
					if (_doNorm) { _mesh.normals = this.normals; }
					else { Debug.LogError("normal flag was set in Apply but MeshBaker didn't generate normals"); }
				}
				
				if (tangents){
					if (_doTan) {_mesh.tangents = this.tangents; }
					else { Debug.LogError("tangent flag was set in Apply but MeshBaker didn't generate tangents"); }
				}
				if (uvs){
					if (_doUV) {_mesh.uv = this.uvs; }
					else { Debug.LogError("uv flag was set in Apply but MeshBaker didn't generate uvs"); }
				}
				if (colors){
					if (_doCol) {_mesh.colors = this.colors; }
					else { Debug.LogError("color flag was set in Apply but MeshBaker didn't generate colors"); }
				}
				if (uv3){
					if (_doUV3) {
						MBVersion.MeshAssignUV3(_mesh,this.uv3s);
					}
					else { Debug.LogError("uv3 flag was set in Apply but MeshBaker didn't generate uv3s"); }	
				}
                if (uv4) {
                    if (_doUV4) {
                        MBVersion.MeshAssignUV4(_mesh, this.uv4s);
                    } else { Debug.LogError("uv4 flag was set in Apply but MeshBaker didn't generate uv4s"); }
                }
                if (uv2){
					if (doUV2()){_mesh.uv2 = this.uv2s; }
					else { Debug.LogError("uv2 flag was set in Apply but lightmapping option was set to " + lightmapOption); }					
				}
				bool do_generate_new_UV2_layout = false;
				if (renderType != MB_RenderType.skinnedMeshRenderer && lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout){
					if (uv2GenerationMethod != null){
						uv2GenerationMethod(_mesh,uv2UnwrappingParamsHardAngle,uv2UnwrappingParamsPackMargin);
						if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log("generating new UV2 layout for the combined mesh ");
					} else {
						Debug.LogError("No GenerateUV2Delegate method was supplied. UV2 cannot be generated.");
					}	
					do_generate_new_UV2_layout = true;
				} else if (renderType == MB_RenderType.skinnedMeshRenderer && lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout) {
					if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("UV2 cannot be generated for SkinnedMeshRenderer objects.");
				}		
				if (renderType != MB_RenderType.skinnedMeshRenderer && lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout && do_generate_new_UV2_layout == false){
					Debug.LogError("Failed to generate new UV2 layout. Only works in editor.");
				}
	
				if (renderType == MB_RenderType.skinnedMeshRenderer){
					if (verts.Length == 0){
						//disable mesh renderer to avoid skinning warning
						targetRenderer.enabled = false;
					} else {
						targetRenderer.enabled = true;
					}
					//needed so that updating local bounds will take affect
					bool uwos = ((SkinnedMeshRenderer) targetRenderer).updateWhenOffscreen;
					((SkinnedMeshRenderer) targetRenderer).updateWhenOffscreen = true;
					((SkinnedMeshRenderer) targetRenderer).updateWhenOffscreen = uwos;
				}
				
				if (bones){
					_mesh.bindposes = this.bindPoses;
					_mesh.boneWeights = this.boneWeights;
				}
                if (triangles || vertices) {
                    if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log("recalculating bounds on mesh.");
                    _mesh.RecalculateBounds();
                }			
			} else {
				Debug.LogError("Need to add objects to this meshbaker before calling Apply or ApplyAll");	
			}
		}
		
		public int[][] GetSubmeshTrisWithShowHideApplied(){
			bool containsHiddenObjects = false;
			for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++){
				if (mbDynamicObjectsInCombinedMesh[i].show == false){
					containsHiddenObjects = true;
					break;
				}
			}
			if (containsHiddenObjects){
				int[] newLengths = new int[submeshTris.Length];
				int[][] newSubmeshTris = new int[submeshTris.Length][];
				for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++){
					MB_DynamicGameObject dgo = mbDynamicObjectsInCombinedMesh[i];
					if (dgo.show){
						for (int j = 0; j < dgo.submeshNumTris.Length; j++){
							newLengths[j] += dgo.submeshNumTris[j];
						}
					}
				}
				for (int i = 0; i < newSubmeshTris.Length; i++){
					newSubmeshTris[i] = new int[newLengths[i]];
				}
				int[] idx = new int[newSubmeshTris.Length];
				for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++){
					MB_DynamicGameObject dgo = mbDynamicObjectsInCombinedMesh[i];
					if (dgo.show){
						for (int j = 0; j < submeshTris.Length; j++){ //for each submesh
							int[] triIdxs = submeshTris[j];
							int startIdx = dgo.submeshTriIdxs[j];
							int endIdx = startIdx + dgo.submeshNumTris[j];
							for (int k = startIdx; k < endIdx; k++){
								newSubmeshTris[j][idx[j]] = triIdxs[k];
								idx[j] = idx[j] + 1;
							}
						}
					}
				}
				return newSubmeshTris;
			} else {
				return submeshTris;
			}
		}
		
		public override void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true,
										bool updateVertices = true, bool updateNormals = true, bool updateTangents = true,
									    bool updateUV = false, bool updateUV2 = false, bool updateUV3 = false, bool updateUV4 = false,
										bool updateColors = false, bool updateSkinningInfo = false){		
			_updateGameObjects(gos, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateColors, updateSkinningInfo);
		}
		
		void _updateGameObjects(GameObject[] gos, bool recalcBounds,
										bool updateVertices, bool updateNormals, bool updateTangents, 
										bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateColors, bool updateSkinningInfo){
			if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("UpdateGameObjects called on " + gos.Length + " objects.");
			_initialize();
			if (_mesh.vertexCount > 0 && _instance2combined_map.Count == 0) {
				Debug.LogWarning("There were vertices in the combined mesh but nothing in the MeshBaker buffers. If you are trying to bake in the editor and modify at runtime, make sure 'Clear Buffers After Bake' is unchecked.");
			}
            MeshChannelsCache meshChannelCache = new MeshChannelsCache(this);
            for (int i = 0; i < gos.Length; i++){
				_updateGameObject(gos[i], updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateColors, updateSkinningInfo, meshChannelCache);
			}
			if (recalcBounds)
				_mesh.RecalculateBounds();
		}

        void _updateGameObject(GameObject go, bool updateVertices, bool updateNormals, bool updateTangents,
                                        bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateColors, bool updateSkinningInfo, MeshChannelsCache meshChannelCache){
			MB_DynamicGameObject dgo = null;
			if (!instance2Combined_MapTryGetValue(go.GetInstanceID(),out dgo)){
				Debug.LogError("Object " + go.name + " has not been added");
				return;
			}
			Mesh mesh = MB_Utility.GetMesh(go);
			if (dgo.numVerts != mesh.vertexCount){
				Debug.LogError("Object " + go.name + " source mesh has been modified since being added. To update it must have the same number of verts");
				return;			
			}

			if (_doUV && updateUV) _copyAndAdjustUVsFromMesh(dgo,mesh,dgo.vertIdx,meshChannelCache);
			if (doUV2() && updateUV2) _copyAndAdjustUV2FromMesh(dgo,mesh,dgo.vertIdx, meshChannelCache);
			if (renderType == MB_RenderType.skinnedMeshRenderer && updateSkinningInfo){
                //only does BoneWeights. Used to do Bones and BindPoses but it doesn't make sence.
                //if updating Bones and Bindposes should remove and re-add
                Renderer r = MB_Utility.GetRenderer(go);
                BoneWeight[] bws = meshChannelCache.GetBoneWeights(r, dgo.numVerts);
                Transform[] bs = _getBones(r);
                //assumes that the bones and boneweights have not been reeordered
                int bwIdx = dgo.vertIdx; //the index in the verts array
                bool switchedBonesDetected = false;
                for (int i = 0; i < bws.Length; i++) {
                    if (bs[bws[i].boneIndex0] != bones[boneWeights[bwIdx].boneIndex0]) {
                        switchedBonesDetected = true;
                        break;
                    }
                    boneWeights[bwIdx].weight0 = bws[i].weight0;
                    boneWeights[bwIdx].weight1 = bws[i].weight1;
                    boneWeights[bwIdx].weight2 = bws[i].weight2;
                    boneWeights[bwIdx].weight3 = bws[i].weight3;
                    bwIdx++;
                }
                if (switchedBonesDetected) {
                    Debug.LogError("Detected that some of the boneweights reference different bones than when initial added. Boneweights must reference the same bones " + dgo.name);
                }
			}
			
			//now do verts, norms, tangents, colors and uv1
			Matrix4x4 l2wMat = go.transform.localToWorldMatrix;
			if (updateVertices){
				Vector3[] nverts = meshChannelCache.GetVertices(mesh);				
				for (int j = 0; j < nverts.Length; j++){
					verts[dgo.vertIdx + j] = l2wMat.MultiplyPoint3x4(nverts[j]);
				}
			}
			l2wMat[0,3] = l2wMat[1,3] = l2wMat[2,3] = 0f;
			if (_doNorm && updateNormals){
				Vector3[] nnorms = meshChannelCache.GetNormals(mesh);
				for (int j = 0; j < nnorms.Length; j++){
                    int vIdx = dgo.vertIdx + j;
                    normals[vIdx] = l2wMat.MultiplyPoint3x4(nnorms[j]);
					normals[vIdx] = normals[vIdx].normalized;					
				}
			}
			if (_doTan && updateTangents){
				Vector4[] ntangs = meshChannelCache.GetTangents(mesh);
				for (int j = 0; j < ntangs.Length; j++){
					int midx = dgo.vertIdx + j;
					float w = ntangs[j].w; //need to preserve the w value
					Vector3 tn = l2wMat.MultiplyPoint3x4(ntangs[j]);
					tn.Normalize();
					tangents[midx] = tn;
					tangents[midx].w = w;					
				}
			}
			if (_doCol && updateColors){
				Color[] ncolors = meshChannelCache.GetColors(mesh);
				for (int j = 0; j < ncolors.Length; j++) colors[dgo.vertIdx + j] = ncolors[j];
			}
			if (_doUV3 && updateUV3){
				Vector2[] nuv3 = meshChannelCache.GetUv3(mesh);
				for (int j = 0; j < nuv3.Length; j++) uv3s[dgo.vertIdx + j] = nuv3[j];
			}
            if (_doUV4 && updateUV4) {
                Vector2[] nuv4 = meshChannelCache.GetUv4(mesh);
                for (int j = 0; j < nuv4.Length; j++) uv4s[dgo.vertIdx + j] = nuv4[j];
            }
        }

		public bool ShowHideGameObjects(GameObject[] toShow, GameObject[] toHide){
			return _showHide(toShow,toHide);	
		}
		
		public override bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource=true){
			int[] delInstanceIDs = null;
			if (deleteGOs != null){
				delInstanceIDs = new int[deleteGOs.Length];
				for (int i = 0; i < deleteGOs.Length; i++){
					if (deleteGOs[i] == null){
						Debug.LogError("The " + i + "th object on the list of objects to delete is 'Null'");	
					}else{
						delInstanceIDs[i] = deleteGOs[i].GetInstanceID();
					}
				}
			}
			return AddDeleteGameObjectsByID(gos,delInstanceIDs,disableRendererInSource);
		}

		public override bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource){
//			Profile.StartProfile("AddDeleteGameObjectsByID");
			if (validationLevel > MB2_ValidationLevel.none){
				//check for duplicates
				if (gos != null){
					for (int i = 0; i < gos.Length; i++){
						if (gos[i] == null){
							Debug.LogError("The " + i + "th object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element.");
							return false;					
						}
						if (validationLevel >= MB2_ValidationLevel.robust){
							for (int j = i + 1; j < gos.Length; j++){
								if (gos[i] == gos[j]){
									Debug.LogError("GameObject " + gos[i] + " appears twice in list of game objects to add");
									return false;
								}
							}
						}
					}
				}
				if (deleteGOinstanceIDs != null && validationLevel >= MB2_ValidationLevel.robust){
					for (int i = 0; i < deleteGOinstanceIDs.Length; i++){			
						for (int j = i + 1; j < deleteGOinstanceIDs.Length; j++){
							if (deleteGOinstanceIDs[i] == deleteGOinstanceIDs[j]){
								Debug.LogError("GameObject " + deleteGOinstanceIDs[i] + "appears twice in list of game objects to delete");
								return false;
							}
						}
					}
				}
			}

			if (_usingTemporaryTextureBakeResult && gos != null && gos.Length > 0){
				MB_Utility.Destroy(_textureBakeResults);
				_textureBakeResults = null;
				_usingTemporaryTextureBakeResult = false;
			}

			//if all objects use the same material we can create a temporary _textureBakeResults 
			if (_textureBakeResults == null && gos != null && gos.Length > 0 && gos[0] != null){
				if (!_CheckIfAllObjsToAddUseSameMaterialsAndCreateTemporaryTextrueBakeResult(gos)){
					return false;
				}
			}


			BuildSceneMeshObject(gos);


			if (!_addToCombined(gos,deleteGOinstanceIDs,disableRendererInSource)){
				Debug.LogError("Failed to add/delete objects to combined mesh");
				return false;
			}
			if (targetRenderer != null){
				if (renderType == MB_RenderType.skinnedMeshRenderer){
					SkinnedMeshRenderer smr = (SkinnedMeshRenderer) targetRenderer;
					smr.bones = bones;
					UpdateSkinnedMeshApproximateBoundsFromBounds();
				}
				targetRenderer.lightmapIndex = GetLightmapIndex();
			}
//			Profile.EndProfile("AddDeleteGameObjectsByID");
//			Profile.PrintResults();
			return true;
		}
		
		public override bool CombinedMeshContains(GameObject go){
			return objectsInCombinedMesh.Contains(go);
		}
		
		public override void ClearBuffers(){
			verts = new Vector3[0];
			normals = new Vector3[0];
			tangents = new Vector4[0];
            uvs = new Vector2[0];
			uv2s = new Vector2[0];
            uv3s = new Vector2[0];
            uv4s = new Vector2[0];
            colors = new Color[0];
			bones = new Transform[0];
			bindPoses = new Matrix4x4[0];
			boneWeights = new BoneWeight[0];
			submeshTris = new int[0][];
			
			mbDynamicObjectsInCombinedMesh.Clear();
			objectsInCombinedMesh.Clear();
			instance2Combined_MapClear();	
			if (_usingTemporaryTextureBakeResult){
				MB_Utility.Destroy(_textureBakeResults);
				_textureBakeResults = null;
				_usingTemporaryTextureBakeResult = false;
			}
			if (LOG_LEVEL >= MB2_LogLevel.trace) MB2_Log.LogDebug("ClearBuffers called");
		}
		
		/*
		 * Empties all channels and clears the mesh
		 */
		public override void ClearMesh(){
			if (_mesh != null){
				MBVersion.MeshClear(_mesh,false);
			} else {
				_mesh = new Mesh();	
			}	
			ClearBuffers();
		}
	
		/*
		 * Empties all channels, destroys the mesh and replaces it with a new mesh
		 */	
		public override void DestroyMesh(){
			if (_mesh != null){
				if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Destroying Mesh");
				MB_Utility.Destroy(_mesh);
			}
			_mesh = new Mesh();
			ClearBuffers();	
		}
		
		public override void DestroyMeshEditor(MB2_EditorMethodsInterface editorMethods){
			if (_mesh != null){
				if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Destroying Mesh");
				editorMethods.Destroy(_mesh);
			}
			_mesh = new Mesh();
			ClearBuffers();
		}	
		
		public bool ValidateTargRendererAndMeshAndResultSceneObj(){
			if (_resultSceneObject == null){
				if (_LOG_LEVEL >= MB2_LogLevel.error) Debug.LogError("Result Scene Object was not set.");
				return false;
			} else {
				if (_targetRenderer == null){
					if (_LOG_LEVEL >= MB2_LogLevel.error) Debug.LogError("Target Renderer was not set.");
					return false;
				} else {
					if (_targetRenderer.transform.parent != _resultSceneObject.transform){
						if (_LOG_LEVEL >= MB2_LogLevel.error) Debug.LogError("Target Renderer game object is not a child of Result Scene Object was not set.");
						return false;
					}
					if (_renderType == MB_RenderType.skinnedMeshRenderer){
						if (!(_targetRenderer is SkinnedMeshRenderer)){
							if (_LOG_LEVEL >= MB2_LogLevel.error) Debug.LogError("Render Type is skinned mesh renderer but Target Renderer is not.");							
							return false;
						}
						if (((SkinnedMeshRenderer)_targetRenderer).sharedMesh != _mesh){
							if (_LOG_LEVEL >= MB2_LogLevel.error) Debug.LogError("Target renderer mesh is not equal to mesh.");
							return false;
						}
					} if (_renderType == MB_RenderType.meshRenderer){
							if (!(_targetRenderer is MeshRenderer)){if (_LOG_LEVEL >= MB2_LogLevel.error) Debug.LogError("Render Type is mesh renderer but Target Renderer is not.");
							return false;
						}
						MeshFilter mf = _targetRenderer.GetComponent<MeshFilter>();
						if (_mesh != mf.sharedMesh){
							if (_LOG_LEVEL >= MB2_LogLevel.error) Debug.LogError("Target renderer mesh is not equal to mesh.");							
							return false;	
						}
					}
				}
			}
			return true;
		}		
		
		public static Renderer BuildSceneHierarch(MB3_MeshCombinerSingle mom, GameObject root, Mesh m, bool createNewChild=false, GameObject[] objsToBeAdded=null){
			if (mom._LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log ("Building Scene Hierarchy createNewChild=" + createNewChild);
			GameObject meshGO;
			MeshFilter mf = null;
			MeshRenderer mr = null;
			SkinnedMeshRenderer smr = null;
			Transform mt = null;
			if (root == null){
				Debug.LogError("root was null.");
				return null;	
			}
			if (mom.textureBakeResults == null){
				Debug.LogError("textureBakeResults must be set.");
				return null;
			}
			if (root.GetComponent<Renderer>() != null){
				Debug.LogError ("root game object cannot have a renderer component");
				return null;
			} 
			if (!createNewChild){
				if (mom.targetRenderer != null){
					mt = mom.targetRenderer.transform;
				} else {
					Renderer[] rs = (Renderer[]) root.GetComponentsInChildren<Renderer>();
					if (rs.Length > 1){
						Debug.LogError("Result Scene Object had multiple child objects with renderers attached. Only one allowed. Try using a game object with no children as the Result Scene Object.");
						return null;
					}
					if (rs.Length == 1){
						if (rs[0].transform.parent != root.transform){
							Debug.LogError("Target Renderer is not an immediate child of Result Scene Object. Try using a game object with no children as the Result Scene Object..");
							return null;						
						}
						mt = rs[0].transform;
					}
				}
			}
 			if (mt != null && mt.parent != root.transform){ //target renderer must be a child of root
				mt = null;
			}
			if (mt == null){
				meshGO = new GameObject(mom.name + "-mesh");
				meshGO.transform.parent = root.transform;
				mt = meshGO.transform;
			}
			meshGO = mt.gameObject;
			if (mom.renderType == MB_RenderType.skinnedMeshRenderer){
				MeshRenderer r = meshGO.GetComponent<MeshRenderer>();
				if (r != null) MB_Utility.Destroy(r);
				MeshFilter f = meshGO.GetComponent<MeshFilter>();
				if (f != null) MB_Utility.Destroy(f);
				smr = meshGO.GetComponent<SkinnedMeshRenderer>();
				if (smr == null) smr = meshGO.AddComponent<SkinnedMeshRenderer>();
			} else {
				SkinnedMeshRenderer r = meshGO.GetComponent<SkinnedMeshRenderer>();
				if (r != null) MB_Utility.Destroy(r);
				mf = meshGO.GetComponent<MeshFilter>();
				if (mf == null) mf = meshGO.AddComponent<MeshFilter>();
				mr = meshGO.GetComponent<MeshRenderer>();
				if (mr == null) mr = meshGO.AddComponent<MeshRenderer>();
			}
			if (mom.textureBakeResults.doMultiMaterial){
				Material[] sharedMats = new Material[mom.textureBakeResults.resultMaterials.Length];
				for (int i = 0; i < sharedMats.Length; i++){
					sharedMats[i] = mom.textureBakeResults.resultMaterials[i].combinedMaterial;
				}
				if (mom.renderType == MB_RenderType.skinnedMeshRenderer){
					smr.sharedMaterial = null;
					smr.sharedMaterials = sharedMats;
					smr.bones = mom.GetBones();
					smr.updateWhenOffscreen = true;
					smr.updateWhenOffscreen = false;
				} else {
					mr.sharedMaterial = null;
					mr.sharedMaterials = sharedMats;
				}
			} else {
				if (mom.renderType == MB_RenderType.skinnedMeshRenderer){
					smr.sharedMaterials = new Material[] {mom.textureBakeResults.resultMaterial};
					smr.sharedMaterial = mom.textureBakeResults.resultMaterial;
					smr.bones = mom.GetBones();
				} else {
					mr.sharedMaterials = new Material[] {mom.textureBakeResults.resultMaterial};;
					mr.sharedMaterial = mom.textureBakeResults.resultMaterial;
				}
			}
			if (mom.renderType == MB_RenderType.skinnedMeshRenderer){
				smr.sharedMesh = m;
				smr.lightmapIndex = mom.GetLightmapIndex();
			} else {
				mf.sharedMesh = m;
				mr.lightmapIndex = mom.GetLightmapIndex();
			}
			if (mom.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping || mom.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout){
				meshGO.isStatic = true;
			}

			//set layer and tag of combined object if all source objs have same layer


			if (objsToBeAdded != null && objsToBeAdded.Length > 0 && objsToBeAdded[0] != null){
				bool tagsAreSame = true;
				bool layersAreSame = true;
				string tag = objsToBeAdded[0].tag;
				int layer = objsToBeAdded[0].layer;
				for (int i = 0; i < objsToBeAdded.Length; i++){
					if (objsToBeAdded[i] != null){
						if (!objsToBeAdded[i].tag.Equals(tag)) tagsAreSame = false;
						if (objsToBeAdded[i].layer != layer) layersAreSame = false;
					}
				}
				if (tagsAreSame){ 
					root.tag = tag;
					meshGO.tag = tag;
				}
				if (layersAreSame){ 
					root.layer = layer;
					meshGO.layer = layer;
				}
			}
			meshGO.transform.parent = root.transform;
			if (mom.renderType == MB_RenderType.skinnedMeshRenderer){
				return smr;	
			}else{
				return mr;
			}			
		}
		
		public void BuildSceneMeshObject(GameObject[] gos=null, bool createNewChild=false){
			if (_resultSceneObject == null){
				_resultSceneObject = new GameObject("CombinedMesh-" + name);
			}
			_targetRenderer = BuildSceneHierarch(this, _resultSceneObject,GetMesh(),createNewChild, gos);
		}		
		
		//tests if a matrix has been mirrored
		bool IsMirrored( Matrix4x4 tm ){
			Vector3 x = tm.GetRow(0);
			Vector3 y = tm.GetRow(1);
			Vector3 z = tm.GetRow(2);
			x.Normalize(); y.Normalize(); z.Normalize();
			float an = Vector3.Dot(Vector3.Cross(x,y),z);
			return an >= 0 ? false : true;
		}

        public override void CheckIntegrity() {
            //check bones.
			if (renderType == MB_RenderType.skinnedMeshRenderer) {
								for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++) {
										MB_DynamicGameObject dgo = mbDynamicObjectsInCombinedMesh [i];
										HashSet<int> usedBonesWeights = new HashSet<int> ();
										HashSet<int> usedBonesIndexes = new HashSet<int> ();
										for (int j = dgo.vertIdx; j < dgo.vertIdx + dgo.numVerts; j++) {
												usedBonesWeights.Add (boneWeights [j].boneIndex0);
												usedBonesWeights.Add (boneWeights [j].boneIndex1);
												usedBonesWeights.Add (boneWeights [j].boneIndex2);
												usedBonesWeights.Add (boneWeights [j].boneIndex3);
										}
										for (int j = 0; j < dgo.indexesOfBonesUsed.Length; j++) {
												usedBonesIndexes.Add (dgo.indexesOfBonesUsed [j]);
										}

										usedBonesIndexes.ExceptWith (usedBonesWeights);
										if (usedBonesIndexes.Count > 0) {
												Debug.LogError ("The bone indexes were not the same. " + usedBonesWeights.Count + " " + usedBonesIndexes.Count);
										}
										for (int j = 0; j < dgo.indexesOfBonesUsed.Length; j++) {
												if (j < 0 || j > bones.Length)
														Debug.LogError ("Bone index was out of bounds.");
										}
										if (renderType == MB_RenderType.skinnedMeshRenderer && dgo.indexesOfBonesUsed.Length < 1)
												Debug.Log ("DGO had no bones");

								}
						}
        }


        //--------  Begin Skinned Mesh Bone Routines ---------------
        /*
        void _UpdateBonesBindPosesAndBoneWeightsFromMesh(GameObject go, MB_DynamicGameObject dgo, int vertsIdx, int bonesIdx) {
            //updates skinning info in the combined mesh
            Renderer r = MB_Utility.GetRenderer(go);
            Transform[] dgoBones = _getBones(r);
            Matrix4x4[] dgoBindPoses = _getBindPoses(r);
            BoneWeight[] dgoBoneWeights = _getBoneWeights(r, dgo.numVerts);

            //build a map src bone idx to combined bone idx map
            //todo make this more efficient use the dgo.indexesOfBonesUsed
            //todo make sure that the bindposes and boneweights are the dame
            int[] srcIndex2combinedIndexMap = new int[dgoBones.Length];
            for (int j = 0; j < dgoBones.Length; j++) {
                for (int k = 0; k < bones.Length; k++) {
                    if (dgoBones[j] == bones[k]) {
                        if (dgoBindPoses[j] == bindPoses[k]) {
                            srcIndex2combinedIndexMap[j] = k;
                            break;
                        }
                    }
                }
            }

            //remap the bone weights for this dgo
            //build a list of usedBones, can't trust dgoBones because it contains all bones in the rig
            //HashSet<int> usedBones = new HashSet<int>();
            for (int j = 0; j < dgoBoneWeights.Length; j++) {
                int newVertIdx = vertsIdx + j;
                nboneWeights[newVertIdx].boneIndex0 = srcIndex2combinedIndexMap[dgoBoneWeights[j].boneIndex0];
                nboneWeights[newVertIdx].boneIndex1 = srcIndex2combinedIndexMap[dgoBoneWeights[j].boneIndex1];
                nboneWeights[newVertIdx].boneIndex2 = srcIndex2combinedIndexMap[dgoBoneWeights[j].boneIndex2];
                nboneWeights[newVertIdx].boneIndex3 = srcIndex2combinedIndexMap[dgoBoneWeights[j].boneIndex3];
                nboneWeights[newVertIdx].weight0 = dgoBoneWeights[j].weight0;
                nboneWeights[newVertIdx].weight1 = dgoBoneWeights[j].weight1;
                nboneWeights[newVertIdx].weight2 = dgoBoneWeights[j].weight2;
                nboneWeights[newVertIdx].weight3 = dgoBoneWeights[j].weight3;
            }
            //dgo._originalBindPoses = dgoBindPoses;
            //dgo._originalBones = dgoBones;


            
            //for (int j = 0; j < nboneWeights.Length; j++) {
            //    nboneWeights[j].boneIndex0 = nboneWeights[j].boneIndex0 + bonesIdx;
            //    nboneWeights[j].boneIndex1 = nboneWeights[j].boneIndex1 + bonesIdx;
            //    nboneWeights[j].boneIndex2 = nboneWeights[j].boneIndex2 + bonesIdx;
            //    nboneWeights[j].boneIndex3 = nboneWeights[j].boneIndex3 + bonesIdx;
            //}
            //nboneWeights.CopyTo(boneWeights, vertsIdx);
            
            if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log("_UpdateBonesBindPosesAndBoneWeightsFromMesh copied " + nboneWeights.Length);
            //return nbindPoses.Length;
        }
        */

        List<MB_DynamicGameObject>[] _buildBoneIdx2dgoMap() {
            List<MB_DynamicGameObject>[] boneIdx2dgoMap = new List<MB_DynamicGameObject>[bones.Length];
            for (int i = 0; i < boneIdx2dgoMap.Length; i++) boneIdx2dgoMap[i] = new List<MB_DynamicGameObject>();
            // build the map of bone indexes to objects that use them
            for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++) {
                MB_DynamicGameObject dgo = mbDynamicObjectsInCombinedMesh[i];
                for (int j = 0; j < dgo.indexesOfBonesUsed.Length; j++) {
                    boneIdx2dgoMap[dgo.indexesOfBonesUsed[j]].Add(dgo);
                }
            }
            return boneIdx2dgoMap;
        }

        void _CollectBonesToAddForDGO(MB_DynamicGameObject dgo, List<int> boneIdxsToDelete, HashSet<BoneAndBindpose> bonesToAdd, Renderer r, MeshChannelsCache meshChannelCache) {
            //compile a list of bone transforms to add
            Matrix4x4[] dgoBindPoses = dgo._tmpCachedBindposes = meshChannelCache.GetBindposes(r);
            Transform[] dgoBones = dgo._tmpCachedBones = _getBones(r);
            //for each bone see if it exists in the bones array
            for (int j = 0; j < dgoBones.Length; j++) {
                bool foundInBonesList = false;
                for (int k = 0; k < bones.Length; k++) {
                    if (boneIdxsToDelete.Contains(k)) {
                        continue;
                    }
                    if (dgoBones[j] == bones[k]) {
                        if (dgoBindPoses[j] == bindPoses[k]) {
                            foundInBonesList = true;
                        }
                        break;
                    }
                }
                if (!foundInBonesList) {
                    BoneAndBindpose bb = new BoneAndBindpose(dgoBones[j], dgoBindPoses[j]);
                    if (!bonesToAdd.Contains(bb)) {
                        bonesToAdd.Add(bb);
                    }
                }
            }
        }

        void _CopyBonesWeAreKeepingToNewBonesArrayAndAdjustBWIndexes(List<int> boneIdxsToDelete, HashSet<BoneAndBindpose> bonesToAdd, Transform[] nbones, Matrix4x4[] nbindPoses, BoneWeight[] nboneWeights, int totalDeleteVerts) {
            //bones are copied separately because some dgos share bones
            if (boneIdxsToDelete.Count > 0) {
                //bones are being moved in bones array so need to do some remapping
                int[] oldBonesIndex2newBonesIndexMap = new int[bones.Length];
                boneIdxsToDelete.Sort();
                int newIdx = 0;
                int indexInDeleteList = 0;

                //bones were deleted so we need to rebuild bones and bind poses
                //and build a map of old bone indexes to new bone indexes
                //do this by copying old to new skipping ones we are deleting
                for (int i = 0; i < bones.Length; i++) {
                    if (indexInDeleteList < boneIdxsToDelete.Count &&
                        boneIdxsToDelete[indexInDeleteList] == i) {
                        //we are deleting this bone so skip its index
                        indexInDeleteList++;
                        oldBonesIndex2newBonesIndexMap[i] = -1;
                    } else {
                        oldBonesIndex2newBonesIndexMap[i] = newIdx;
                        nbones[newIdx] = bones[i];
                        nbindPoses[newIdx] = bindPoses[i];
                        newIdx++;
                    }
                }
                //adjust the indexes on the boneWeights
                int numVertKeeping = boneWeights.Length - totalDeleteVerts;
                for (int i = 0; i < numVertKeeping; i++) {
                    nboneWeights[i].boneIndex0 = oldBonesIndex2newBonesIndexMap[nboneWeights[i].boneIndex0];
                    nboneWeights[i].boneIndex1 = oldBonesIndex2newBonesIndexMap[nboneWeights[i].boneIndex1];
                    nboneWeights[i].boneIndex2 = oldBonesIndex2newBonesIndexMap[nboneWeights[i].boneIndex2];
                    nboneWeights[i].boneIndex3 = oldBonesIndex2newBonesIndexMap[nboneWeights[i].boneIndex3];
                }
                //adjust the bone indexes on the dgos from old to new
                for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++) {
                    MB_DynamicGameObject dgo = mbDynamicObjectsInCombinedMesh[i];
                    for (int j = 0; j < dgo.indexesOfBonesUsed.Length; j++) {
                        dgo.indexesOfBonesUsed[j] = oldBonesIndex2newBonesIndexMap[dgo.indexesOfBonesUsed[j]];
                    }
                }
            } else { //no bones are moving so can simply copy bones from old to new
                Array.Copy(bones, nbones, bones.Length);
                Array.Copy(bindPoses, nbindPoses, bindPoses.Length);
            }
        }

        void _AddBonesToNewBonesArrayAndAdjustBWIndexes(MB_DynamicGameObject dgo, Renderer r, int vertsIdx,
                                                         Transform[] nbones, BoneWeight[] nboneWeights, MeshChannelsCache meshChannelCache) {
            //Renderer r = MB_Utility.GetRenderer(go);
            Transform[] dgoBones = dgo._tmpCachedBones;
            Matrix4x4[] dgoBindPoses = dgo._tmpCachedBindposes;
            BoneWeight[] dgoBoneWeights = meshChannelCache.GetBoneWeights(r, dgo.numVerts);
            //build a map src bone idx to combined bone idx map

            int[] srcIndex2combinedIndexMap = new int[dgoBones.Length];
            for (int j = 0; j < dgoBones.Length; j++) {
                for (int k = 0; k < nbones.Length; k++) {
                    if (dgoBones[j] == nbones[k]) {
                        if (dgoBindPoses[j] == bindPoses[k]) {
                            srcIndex2combinedIndexMap[j] = k;
                            break;
                        }
                    }
                }
            }
            //remap the bone weights for this dgo
            //build a list of usedBones, can't trust dgoBones because it contains all bones in the rig
            HashSet<int> usedBones = new HashSet<int>();
            for (int j = 0; j < dgoBoneWeights.Length; j++) {
                int newVertIdx = vertsIdx + j;
                nboneWeights[newVertIdx].boneIndex0 = srcIndex2combinedIndexMap[dgoBoneWeights[j].boneIndex0];
                nboneWeights[newVertIdx].boneIndex1 = srcIndex2combinedIndexMap[dgoBoneWeights[j].boneIndex1];
                nboneWeights[newVertIdx].boneIndex2 = srcIndex2combinedIndexMap[dgoBoneWeights[j].boneIndex2];
                nboneWeights[newVertIdx].boneIndex3 = srcIndex2combinedIndexMap[dgoBoneWeights[j].boneIndex3];
                nboneWeights[newVertIdx].weight0 = dgoBoneWeights[j].weight0;
                nboneWeights[newVertIdx].weight1 = dgoBoneWeights[j].weight1;
                nboneWeights[newVertIdx].weight2 = dgoBoneWeights[j].weight2;
                nboneWeights[newVertIdx].weight3 = dgoBoneWeights[j].weight3;
                usedBones.Add(nboneWeights[newVertIdx].boneIndex0);
                usedBones.Add(nboneWeights[newVertIdx].boneIndex1);
                usedBones.Add(nboneWeights[newVertIdx].boneIndex2);
                usedBones.Add(nboneWeights[newVertIdx].boneIndex3);
            }
            //dgo._originalBindPoses = dgoBindPoses;
            //dgo._originalBones = dgoBones;
            dgo.indexesOfBonesUsed = new int[usedBones.Count];
            usedBones.CopyTo(dgo.indexesOfBonesUsed);
            dgo._tmpCachedBones = null;
            dgo._tmpCachedBindposes = null;

            //check original bones and bindPoses
            /*
            for (int j = 0; j < dgo.indexesOfBonesUsed.Length; j++) {
                Transform bone = bones[dgo.indexesOfBonesUsed[j]];
                Matrix4x4 bindpose = bindPoses[dgo.indexesOfBonesUsed[j]];
                bool found = false;
                for (int k = 0; k < dgo._originalBones.Length; k++) {
                    if (dgo._originalBones[k] == bone && dgo._originalBindPoses[k] == bindpose) {
                        found = true;
                    }
                }
                if (!found) Debug.LogError("A Mismatch between original bones and bones array. " + dgo.name);
            }
            */
        }
        //--------  End Skinned Mesh Bone Routines -----------------


    }
}