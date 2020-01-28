...
    void Start()
    {
        if (item.isStackable)
        {
            itemAmount = int.Parse(transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        }

        slotIndex = transform.parent.GetSiblingIndex();
        itemImage = GetComponent<Image>();
        tempParent = UI.instance.gameObject.transform;
        previousParent = transform.parent;
    }

    void Update()
    {
        if (isBeingDragged)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    public void OnAmountChanged(int change, bool positive)
    {
        amountText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (positive)
        {
            amountText.text = (itemAmount += change).ToString();
        }
        else
        {
            amountText.text = (itemAmount -= change).ToString();
        }
        if(itemAmount == 0)
        {
            Inventory.instance.DestoryItemFromInventory(slotIndex);
        }
    }

    public void OnAmountSplitted(int change)
    {
        amountText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        amountText.text = change.ToString();
    }

    public void RefuseItemMove(bool isInventory)
    {
        if (isInventory)
        {
            transform.parent = previousParent;
            transform.position = Inventory.instance.inventorySlots[slotIndex].transform.position;
        }
        else
        {
            transform.parent = previousParent;
            transform.position = previousParent.transform.position;
        }
    }

    public void SplitItemStack()
    {
        if (item.isStackable)
        {
            if (Inventory.instance.CheckIfInventoryHasSpace())
            {
                double thisObjectAmount = Math.Ceiling((double)itemAmount / 2);
                double otherObjectAmount = Math.Floor((double)itemAmount / 2);

                itemAmount = (int)thisObjectAmount;
                Inventory.instance.AddNewStackToInventory(gameObject, (int)otherObjectAmount);
                OnAmountSplitted((int)thisObjectAmount);
            }
        }
    }

    public void ItemDraggedToInventory()
    {
        if (!item.isStackable)
        {
            if (Inventory.instance.CheckIfInventoryHasSpace())
            {
                if (UI.instance.mouseOverSlot.transform.childCount > 0 && !UI.instance.mouseOverSlot.gameObject.name.Equals(transform.parent.gameObject.name))
                {
                    if (previousParent.name.Contains("Inventory"))
                    {
                        GameObject child = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                        child.GetComponent<ItemHandler>().slotIndex = slotIndex;
                        child.transform.parent = Inventory.instance.inventorySlots[slotIndex].transform;
                        child.transform.position = child.transform.parent.transform.position;

                        slotIndex = UI.instance.mouseOverSlot.transform.GetSiblingIndex();
                        transform.parent = Inventory.instance.inventorySlots[slotIndex].transform;
                        transform.position = transform.parent.position;
                    }
                    else if (previousParent.name.Contains("Store"))
                    {
                        RefuseItemMove(false);
                    }
                    else if (previousParent.name.Contains("Bank"))
                    {
                        GameObject child = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                        child.GetComponent<ItemHandler>().slotIndex = Bank.instance.bankSlots[slotIndex].transform.GetSiblingIndex();
                        child.transform.parent = Bank.instance.bankSlots[slotIndex].transform;
                        child.transform.position = child.transform.parent.transform.position;

                        slotIndex = UI.instance.mouseOverSlot.transform.GetSiblingIndex();
                        transform.parent = Inventory.instance.inventorySlots[slotIndex].transform;
                        transform.position = transform.parent.position;
                    }
                    else if(previousParent.name.Contains("Equipment"))
                    {
                        if (UI.instance.mouseOverSlot.transform.GetChild(0).GetComponent<ItemHandler>().item.equipmentSlot == item.equipmentSlot)
                        {
                            GameObject child = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                            child.GetComponent<ItemHandler>().slotIndex = slotIndex;
                            child.transform.parent = GameObject.Find("Equipment" + item.equipmentSlot).transform;
                            child.transform.position = child.transform.parent.transform.position;

                            slotIndex = UI.instance.mouseOverSlot.transform.GetSiblingIndex();
                            transform.parent = UI.instance.mouseOverSlot.transform.parent.GetChild(slotIndex);
                            transform.position = transform.parent.position;
                        }
                        else
                        {
                            RefuseItemMove(false);
                        }
                    }
                }
                else
                {
                    slotIndex = UI.instance.mouseOverSlot.transform.GetSiblingIndex();
                    transform.parent = Inventory.instance.inventorySlots[slotIndex].transform;
                    transform.position = transform.parent.position;

                    if (previousParent.name.Contains("Store"))
                    {
                        int price;
                        bool ableToBuy = Store.instance.IsAbleToBuy(item.value, itemAmount, out price);
                        if (ableToBuy)
                        {
                            Inventory.instance.RemoveItemFromInventory("Coins", price);
                        }
                        else
                        {
                            RefuseItemMove(false);
                        }
                    }
                    else if (previousParent.name.Contains("Equipment"))
                    {
                        Equipment.instance.ShowDefaultEquipmentMesh(item);
                    }
                }
            }
            else
            {
                if (previousParent.name.Contains("Equipment"))
                {
                    if (UI.instance.mouseOverSlot.transform.GetChild(0).GetComponent<ItemHandler>().item.equipmentSlot == item.equipmentSlot)
                    {
                        GameObject child = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                        child.GetComponent<ItemHandler>().slotIndex = slotIndex;
                        child.transform.parent = GameObject.Find("Equipment" + item.equipmentSlot).transform;
                        child.transform.position = child.transform.parent.transform.position;

                        slotIndex = UI.instance.mouseOverSlot.transform.GetSiblingIndex();
                        transform.parent = UI.instance.mouseOverSlot.transform.parent.GetChild(slotIndex);
                        transform.position = transform.parent.position;
                    }
                    else
                    {
                        RefuseItemMove(false);
                    }
                }
                else if(previousParent.name.Contains("Store"))
                {
                    RefuseItemMove(false);
                }
                else if(previousParent.name.Contains("Bank"))
                {
                    GameObject child = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                    child.GetComponent<ItemHandler>().slotIndex = Bank.instance.bankSlots[slotIndex].transform.GetSiblingIndex();
                    child.transform.parent = Bank.instance.bankSlots[slotIndex].transform;
                    child.transform.position = child.transform.parent.transform.position;

                    slotIndex = UI.instance.mouseOverSlot.transform.GetSiblingIndex();
                    transform.parent = Inventory.instance.inventorySlots[slotIndex].transform;
                    transform.position = transform.parent.position;
                }
            }
        }
        else
        {
            if (Inventory.instance.CheckIfInventoryHasSpace())
            {
                if (UI.instance.mouseOverSlot.transform.childCount > 0 && !UI.instance.mouseOverSlot.gameObject.name.Equals(transform.parent.gameObject.name))
                {
                    if (previousParent.name.Contains("Inventory"))
                    {
                        if (UI.instance.mouseOverSlot.transform.GetChild(0).name.Equals(item.title))
                        {
                            UI.instance.mouseOverSlot.transform.GetChild(0).GetComponent<ItemHandler>().OnAmountChanged(itemAmount, true);
                            Destroy(gameObject);
                        }
                        else
                        {
                            GameObject child = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                            child.GetComponent<ItemHandler>().slotIndex = slotIndex;
                            child.transform.parent = Inventory.instance.inventorySlots[slotIndex].transform;
                            child.transform.position = child.transform.parent.transform.position;

                            slotIndex = UI.instance.mouseOverSlot.transform.GetSiblingIndex();
                            transform.parent = Inventory.instance.inventorySlots[slotIndex].transform;
                            transform.position = transform.parent.position;
                        }
                    }
                    else if (previousParent.name.Contains("Store"))
                    {
                        GameObject itemInInventorySlot = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                        GameObject itemInStoreSlot = UI.instance.transform.GetChild(UI.instance.transform.childCount - 1).gameObject;
                        if (itemInInventorySlot.name.Equals(itemInStoreSlot.name))
                        {
                            itemInInventorySlot.GetComponent<ItemHandler>().OnAmountChanged(itemAmount, true);
                            Destroy(itemInStoreSlot);
                            if (previousParent.name.Contains("Store"))
                            {
                                int price;
                                bool ableToBuy = Store.instance.IsAbleToBuy(item.value, itemAmount, out price);
                                if (ableToBuy)
                                {
                                    Inventory.instance.RemoveItemFromInventory("Coins", price);
                                }
                                else
                                {
                                    RefuseItemMove(false);
                                }
                            }
                        }
                        else
                        {
                            RefuseItemMove(false);
                        }
                    }
                    else if (previousParent.name.Contains("Bank"))
                    {
                        GameObject itemInInventorySlot = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                        GameObject itemInBankSlot = UI.instance.transform.GetChild(UI.instance.transform.childCount - 1).gameObject;
                        if (itemInInventorySlot.name.Equals(itemInBankSlot.name))
                        {
                            itemInInventorySlot.GetComponent<ItemHandler>().OnAmountChanged(itemAmount, true);
                            Destroy(itemInBankSlot);
                        }
                        else
                        {
                            GameObject child = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                            child.GetComponent<ItemHandler>().slotIndex = Bank.instance.bankSlots[slotIndex].transform.GetSiblingIndex();
                            child.transform.parent = Bank.instance.bankSlots[slotIndex].transform;
                            child.transform.position = child.transform.parent.transform.position;

                            slotIndex = UI.instance.mouseOverSlot.transform.GetSiblingIndex();
                            transform.parent = Inventory.instance.inventorySlots[slotIndex].transform;
                            transform.position = transform.parent.position;
                        }
                    }
                    else if (previousParent.name.Contains("Equipment"))
                    {
                        if (UI.instance.mouseOverSlot.transform.GetChild(0).GetComponent<ItemHandler>().item.equipmentSlot == item.equipmentSlot)
                        {
                            GameObject itemInInventorySlot = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                            GameObject itemInEquipmentSlot = UI.instance.transform.GetChild(UI.instance.transform.childCount - 1).gameObject;
                            if (!itemInInventorySlot.name.Equals(itemInEquipmentSlot.name))
                            {
                                if (Inventory.instance.CheckIfInventoryContainsItem(itemInEquipmentSlot.name.Split('(')[0], out int index))
                                {
                                    GameObject foundOne = Inventory.instance.inventorySlots[index].transform.GetChild(0).gameObject;
                                    foundOne.GetComponent<ItemHandler>().OnAmountChanged(itemAmount, true);
                                    Destroy(itemInEquipmentSlot);
                                }
                                else
                                {
                                    GameObject child = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                                    child.GetComponent<ItemHandler>().slotIndex = slotIndex;
                                    child.transform.parent = GameObject.Find("Equipment" + item.equipmentSlot).transform;
                                    child.transform.position = child.transform.parent.transform.position;

                                    slotIndex = UI.instance.mouseOverSlot.transform.GetSiblingIndex();
                                    transform.parent = UI.instance.mouseOverSlot.transform.parent.GetChild(slotIndex);
                                    transform.position = transform.parent.position;
                                }
                            }
                            else
                            {
                                itemInInventorySlot.GetComponent<ItemHandler>().OnAmountChanged(itemAmount, true);
                                Destroy(itemInEquipmentSlot);
                            }
                        }
                        else
                        {
                            RefuseItemMove(false);
                        }
                    }
                }
                else
                {
                    int index;
                    if (Inventory.instance.CheckIfInventoryContainsItem(item.title, out index))
                    {
                        if(previousParent.name.Contains("Inventory"))
                        {
                            slotIndex = UI.instance.mouseOverSlot.transform.GetSiblingIndex();
                            transform.parent = Inventory.instance.inventorySlots[slotIndex].transform;
                            transform.position = transform.parent.position;
                        }
                        else
                        {
                            GameObject itemInInventorySlot = Inventory.instance.inventorySlots[index].transform.GetChild(0).gameObject;
                            GameObject itemInOtherSlot = UI.instance.transform.GetChild(UI.instance.transform.childCount - 1).gameObject;
                            itemInInventorySlot.GetComponent<ItemHandler>().OnAmountChanged(itemAmount, true);
                            Destroy(itemInOtherSlot);
                        }
                        if (previousParent.name.Contains("Store"))
                        {
                            int price;
                            bool ableToBuy = Store.instance.IsAbleToBuy(item.value, itemAmount, out price);
                            if (ableToBuy)
                            {
                                Inventory.instance.RemoveItemFromInventory("Coins", price);
                            }
                            else
                            {
                                RefuseItemMove(false);
                            }
                        }
                        else if (previousParent.name.Contains("Equipment"))
                        {
                            Equipment.instance.ShowDefaultEquipmentMesh(item);
                        }
                    }
                    else
                    {
                        slotIndex = UI.instance.mouseOverSlot.transform.GetSiblingIndex();
                        transform.parent = Inventory.instance.inventorySlots[slotIndex].transform;
                        transform.position = transform.parent.position;
                        if (previousParent.name.Contains("Store"))
                        {
                            int price;
                            bool ableToBuy = Store.instance.IsAbleToBuy(item.value, itemAmount, out price);
                            if (ableToBuy)
                            {
                                Inventory.instance.RemoveItemFromInventory("Coins", price);
                            }
                            else
                            {
                                RefuseItemMove(false);
                            }
                        }
                        else if (previousParent.name.Contains("Equipment"))
                        {
                            Equipment.instance.ShowDefaultEquipmentMesh(item);
                        }
                    }
                }
            }
            else
            {
                GameObject itemInInventorySlot = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                GameObject itemInOtherSlot = UI.instance.transform.GetChild(UI.instance.transform.childCount - 1).gameObject;
                if(itemInInventorySlot.name.Equals(itemInOtherSlot.name))
                {
                    itemInInventorySlot.GetComponent<ItemHandler>().OnAmountChanged(itemAmount, true);
                    Destroy(itemInOtherSlot);
                }
                else
                {
                    int index;
                    if (Inventory.instance.CheckIfInventoryContainsItem(item.title, out index))
                    {
                        itemInInventorySlot = Inventory.instance.inventorySlots[index].transform.GetChild(0).gameObject;
                        itemInInventorySlot.GetComponent<ItemHandler>().OnAmountChanged(itemAmount, true);
                        Destroy(itemInOtherSlot);
                    }
                    else
                    {
                        if (previousParent.name.Contains("Equipment"))
                        {
                            if (UI.instance.mouseOverSlot.transform.GetChild(0).GetComponent<ItemHandler>().item.equipmentSlot == item.equipmentSlot)
                            {
                                GameObject child = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                                child.GetComponent<ItemHandler>().slotIndex = slotIndex;
                                child.transform.parent = GameObject.Find("Equipment" + item.equipmentSlot).transform;
                                child.transform.position = child.transform.parent.transform.position;

                                slotIndex = UI.instance.mouseOverSlot.transform.GetSiblingIndex();
                                transform.parent = UI.instance.mouseOverSlot.transform.parent.GetChild(slotIndex);
                                transform.position = transform.parent.position;
                            }
                            else
                            {
                                RefuseItemMove(false);
                            }
                        }
                        else if(previousParent.name.Contains("Store"))
                        {
                            RefuseItemMove(false);
                        }
                        else if(previousParent.name.Contains("Bank"))
                        {
                            GameObject child = UI.instance.mouseOverSlot.transform.GetChild(0).gameObject;
                            child.GetComponent<ItemHandler>().slotIndex = Bank.instance.bankSlots[slotIndex].transform.GetSiblingIndex();
                            child.transform.parent = Bank.instance.bankSlots[slotIndex].transform;
                            child.transform.position = child.transform.parent.transform.position;

                            slotIndex = UI.instance.mouseOverSlot.transform.GetSiblingIndex();
                            transform.parent = Inventory.instance.inventorySlots[slotIndex].transform;
                            transform.position = transform.parent.position;
                        }
                    }
                }
            }
        }
    }

    public void ItemClickedToInventory()
    {
        if (!item.isStackable)
        {
            if (Inventory.instance.CheckIfInventoryHasSpace())
            {
                slotIndex = Inventory.instance.GetFirstEmptySlot();
                transform.parent = Inventory.instance.inventorySlots[slotIndex].transform;
                transform.position = transform.parent.position;

                if (previousParent.name.Contains("Store"))
                {
                    int price;
                    bool ableToBuy = Store.instance.IsAbleToBuy(item.value, itemAmount, out price);
                    if (ableToBuy)
                    {
                        Inventory.instance.RemoveItemFromInventory("Coins", price);
                    }
                    else
                    {
                        RefuseItemMove(false);
                    }
                }
                else if(previousParent.name.Contains("Equipment"))
                {
                    Equipment.instance.ShowDefaultEquipmentMesh(item);
                }
            }
        }
        else
        {
            if (Inventory.instance.CheckIfInventoryHasSpace())
            {
                int index;
                if (Inventory.instance.CheckIfInventoryContainsItem(item.title, out index))
                {
                    GameObject itemInBankSlot = Inventory.instance.inventorySlots[index].transform.GetChild(0).gameObject;
                    GameObject itemInInventorySlot = gameObject;
                    itemInBankSlot.GetComponent<ItemHandler>().OnAmountChanged(itemAmount, true);
                    Destroy(itemInInventorySlot);

                    if (previousParent.name.Contains("Store"))
                    {
                        int price;
                        bool ableToBuy = Store.instance.IsAbleToBuy(item.value, itemAmount, out price);
                        if (ableToBuy)
                        {
                            Inventory.instance.RemoveItemFromInventory("Coins", price);
                        }
                        else
                        {
                            RefuseItemMove(false);
                        }
                    }
                    else if (previousParent.name.Contains("Equipment"))
                    {
                        Equipment.instance.ShowDefaultEquipmentMesh(item);
                    }
                }
                else
                {
                    slotIndex = Inventory.instance.GetFirstEmptySlot();
                    transform.parent = Inventory.instance.inventorySlots[slotIndex].transform;
                    transform.position = transform.parent.position;

                    if (previousParent.name.Contains("Store"))
                    {
                        int price;
                        bool ableToBuy = Store.instance.IsAbleToBuy(item.value, itemAmount, out price);
                        if (ableToBuy)
                        {
                            Inventory.instance.RemoveItemFromInventory("Coins", price);
                        }
                        else
                        {
                            RefuseItemMove(false);
                        }
                    }
                    else if (previousParent.name.Contains("Equipment"))
                    {
                        Equipment.instance.ShowDefaultEquipmentMesh(item);
                    }
                }
            }
            else
            {
                int index;
                if (Inventory.instance.CheckIfInventoryContainsItem(item.title, out index))
                {
                    GameObject itemInBankSlot = Inventory.instance.inventorySlots[index].transform.GetChild(0).gameObject;
                    GameObject itemInInventorySlot = gameObject;
                    itemInBankSlot.GetComponent<ItemHandler>().OnAmountChanged(itemAmount, true);
                    Destroy(itemInInventorySlot);

                    if (previousParent.name.Contains("Store"))
                    {
                        int price;
                        bool ableToBuy = Store.instance.IsAbleToBuy(item.value, itemAmount, out price);
                        if (ableToBuy)
                        {
                            Inventory.instance.RemoveItemFromInventory("Coins", price);
                        }
                        else
                        {
                            RefuseItemMove(false);
                        }
                    }
                }
            }
        }
    }
...
