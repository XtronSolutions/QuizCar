using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Sdkbox;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class ShopManager : SingeltonBase<ShopManager>
{
    //private Sdkbox.IAP iap;
    public delegate void Callback(string pkg);
    Callback caller;

    public Text messageText;
    public Text restoreText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PurchasePackage(string pkg, Callback OnPurchase)
    {
       
            if (Application.internetReachability != NetworkReachability.NotReachable)
                MenusUI.Intance.progressScreen.SetActive(true);
            caller = null;
            caller = OnPurchase;
            log("About to purchase " + pkg);
            Debug.Log("About to purchase "+pkg);
        caller(pkg);
        MenusUI.Intance.progressScreen.SetActive(false);



        //  UnityIAPManager.Instance.Purchase(pkg);

    }
    public void PurchaseCharacter(string characterName,Callback OnPurchase)
    {
       // if (iap != null)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
                MenusUI.Intance.progressScreen.SetActive(true);
            caller = null;
            caller = OnPurchase;
            log("About to purchase character");
          //  caller(characterName);
            //MenuManager.Instance.waiting.SetActive(false);
            // iap.purchase("Enes");
             UnityIAPManager.Instance.Purchase(characterName);
            // caller("girl");
        }
    }
    public void Restore()
    {
      //  if (iap != null)
        {
            caller = null;
            if (Application.internetReachability != NetworkReachability.NotReachable)
                MenusUI.Intance.progressScreen.SetActive(true);

            restoreText.text = "Restoring purchases...";
            restoreText.gameObject.SetActive(true);
            UnityIAPManager.Instance.RestorePurchases();
            
          //  iap.restore();

           
        }
    }


    //public void onInitialized(bool status)
    //{
    //    log("Init " + status);
    //}

    //public void onSuccess(Product product)
    //{
    //    MenuManager.Instance.waiting.SetActive(false);
    //    caller(product.name);
    //    log("onSuccess: " + product.name);
    //}

    //public void onFailure(Product product, string message)
    //{
    //    MenuManager.Instance.waiting.SetActive(false);
    //    log("onFailure " + message);
    //}

    //public void onCanceled(Product product)
    //{
    //    MenuManager.Instance.waiting.SetActive(false);
    //    log("onCanceled product: " + product.name);
    //}

    //public void onRestored(Product product)
    //{
    //    log("onRestored: " + product.name);
    //    MenuManager.Instance.waiting.SetActive(false);
    //    if (product.name == "Enes")
    //    {
    //        MenuManager.Instance.CharacterSelectionMenu.GetComponent<CharacterSeletionMenuListner>().OnCharacterPurchased("Enes");
    //        restoreText.text = "Purchases restored successfully";
    //        restoreText.gameObject.SetActive(true);
    //    }
    //}
       

    //public void onProductRequestSuccess(Product[] products)
    //{
    //    MenuManager.Instance.waiting.SetActive(false);
    //    foreach (var p in products)
    //    {
    //        log("Product: " + p.name + " price: " + p.price);
    //    }
    //}

    //public void onProductRequestFailure(string message)
    //{
    //    MenuManager.Instance.waiting.SetActive(false);
    //    log("onProductRequestFailure: " + message);
    //}

    //public void onRestoreComplete(bool bResut, string message)
    //{
    //    MenuManager.Instance.waiting.SetActive(false);
    //    log("onRestoreComplete: " +bResut + " - "+ message);
    //}
    private void log(string msg)
    {

        if (messageText)
        {
           /// messageText.text += msg+"\n";
        }

        Debug.Log(msg);
    }





    public void onInitialized(bool status)
    {
        log("Init " + status);
    }

    public void onSuccess(PurchaseEventArgs product)
    {
        MenusUI.Intance.progressScreen.SetActive(false);
        if (caller != null)
        {
            caller(UnityIAPManager.Instance.GetProductNameByID(product.purchasedProduct.definition.id));

        }
        else
        {
           // if( UnityIAPManager.Instance.GetProductNameByID(product.purchasedProduct.definition.id) == "Enes")
            {
              //  MenuManager.Instance.CharacterSelectionMenu.GetComponent<CharacterSeletionMenuListner>().OnCharacterPurchased(UnityIAPManager.Instance.GetProductNameByID(product.purchasedProduct.definition.id));
                restoreText.text = "Purchases restored successfully";
                restoreText.gameObject.SetActive(true);
            }
            onRestored(product.purchasedProduct.definition.id);
        }
        log("onSuccess: " + product.purchasedProduct.definition.id);
    }

    public void onFailure(Product product, PurchaseFailureReason failureReason)
    {
        MenusUI.Intance.progressScreen.SetActive(false);
        log("onFailure " + product.definition.id);
    }

    public void onCanceled(string product)
    {
        MenusUI.Intance.progressScreen.SetActive(false);
        log("onCanceled product: " + product);
    }

    public void onRestored(string product)
    {
        log("onRestored: " + product);
        MenusUI.Intance.progressScreen.SetActive(false);
        //if (product == "Enes")
        //{
           // MenuManager.Instance.CharacterSelectionMenu.GetComponent<CharacterSeletionMenuListner>().OnCharacterPurchased("Enes");
        //    restoreText.text = "Purchases restored successfully";
          //  restoreText.gameObject.SetActive(true);
        //}
    }


    public void onProductRequestSuccess(Product[] products)
    {
        MenusUI.Intance.progressScreen.SetActive(false);
        foreach (var p in products)
        {
            log("Product: " + p.definition.id + " price: " + p.metadata.localizedPrice);
        }
    }

    public void onProductRequestFailure(string message)
    {
        MenusUI.Intance.progressScreen.SetActive(false);
        log("onProductRequestFailure: " + message);
    }

    public void onRestoreComplete(bool bResut, string message)
    {
        MenusUI.Intance.progressScreen.SetActive(false);
        log("onRestoreComplete: " + bResut + " - " + message);
    }
}