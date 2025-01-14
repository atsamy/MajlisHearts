﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;
//using Unity.Services.Core;
//using Unity.Services.Core.Environments;

public class Purchaser : MonoBehaviour, IDetailedStoreListener
{

    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    public string[] HCProductID;

    internal int[] HCAmount;

    public delegate void GetPrices(string[] prices,int[] amount);
    public GetPrices OnGetPrices;

    internal string[] Prices;

    public static Purchaser Instance;

    Action purchaseComplete;
    internal int[] GemsPrices;


    public string environment = "production";

    void Start()
    {

            Instance = this;
            HCAmount = new int[HCProductID.Length];

            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }

    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (var item in HCProductID)
        {
            builder.AddProduct(item, ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);



        StartCoroutine(WaitForPrices());

    }

    IEnumerator WaitForPrices()
    {
        while (m_StoreController == null)
        {
            yield return new WaitForSeconds(0.2f);
        }

        Prices = new string[HCProductID.Length];
        //Amounts = new int[HCProductID.Length];

        for (int i = 0; i < HCProductID.Length; i++)
        {
            Prices[i] = m_StoreController.products.WithID(HCProductID[i]).metadata.localizedPriceString;
            int amount = 0;
            if (int.TryParse(m_StoreController.products.WithID(HCProductID[i]).metadata.localizedDescription, out amount))
            {
                HCAmount[i] = amount;
            }
            else
            {
                HCAmount[i] = 500;
            }
        }

        if (OnGetPrices != null)
        {
            OnGetPrices(Prices,HCAmount);
        }
    }

    public void GetAllPrices(GetPrices getPrices)
    {
        if (m_StoreController != null)
        {
            Prices = new string[HCProductID.Length];

            for (int i = 0; i < HCProductID.Length; i++)
            {
                Prices[i] = m_StoreController.products.WithID(HCProductID[i]).metadata.localizedPriceString;
            }

            getPrices(Prices,HCAmount);
        }
        else
        {
            OnGetPrices = getPrices;
        }
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyCurrency(int Index, Action onComplete)
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(HCProductID[Index]);

        purchaseComplete = onComplete;
    }

    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);


            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result,message) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        for (int i = 0; i < HCProductID.Length; i++)
        {
            if (String.Equals(args.purchasedProduct.definition.id, HCProductID[i], StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
                purchaseComplete?.Invoke();
            }
        }

        // Or ... an unknown product has been purchased by this user. Fill in additional products here....

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        //throw new NotImplementedException();
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error.ToString() + message);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        //throw new NotImplementedException();
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureDescription));
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        //throw new NotImplementedException();
    }
}
