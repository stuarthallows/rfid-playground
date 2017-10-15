namespace RfidConsole
{
    /// <summary>
    /// Represents Indy Host Library interface packet types
    /// </summary>
    /// <remarks>
    /// See "Host Interface Packet Definitions" document.
    /// </remarks>
    public enum PacketType
    {
        // Common class

        /// <summary>
        /// RFID_PACKET_TYPE_COMMAND_BEGIN
        /// </summary>
        CommandBegin = 0x0000,

        /// <summary>
        /// RFID_PACKET_TYPE_COMMAND_END
        /// </summary>
        CommandEnd = 0x0001,

        /// <summary>
        /// RFID_PACKET_TYPE_ANTENNA_CYCLE_BEGIN
        /// </summary>
        AntennaCycleBegin = 0x0002,

        /// <summary>
        /// RFID_PACKET_TYPE_ANTENNA_BEGIN 
        /// </summary>
        AntennaBegin = 0x0003,

        /// <summary>
        /// RFID_PACKET_TYPE_18K6C_INVENTORY_ROUND_BEGIN
        /// </summary>
        InventoryRoundBegin = 0x0004,

        /// <summary>
        /// RFID_PACKET_TYPE_18K6C_INVENTORY
        /// </summary>
        Inventory = 0x0005,

        /// <summary>
        /// RFID_PACKET_TYPE_18K6C_TAG_ACCESS
        /// </summary>
        TagAccess = 0x0006,

        /// <summary>
        /// RFID_PACKET_TYPE_ANTENNA_CYCLE_END
        /// </summary>
        AntennaCycleEnd = 0x0007,

        /// <summary>
        /// RFID_PACKET_TYPE_ANTENNA_END
        /// </summary>
        AntennaEnd = 0x0008,

        /// <summary>
        /// RFID_PACKET_TYPE_18K6C_INVENTORY_ROUND_END
        /// </summary>
        InventoryRoundEnd = 0x0009,

        /// <summary>
        /// RFID_PACKET_TYPE_INVENTORY_CYCLE_BEGIN
        /// </summary>
        InventoryCycleBegin = 0x000A,

        /// <summary>
        /// RFID_PACKET_TYPE_INVENTORY_CYCLE_END
        /// </summary>
        InventoryCycleEnd = 0x000B,

        /// <summary>
        /// RFID_PACKET_TYPE_CARRIER_INFO
        /// </summary>
        CarrierInfo = 0x000C,

        /// <summary>
        /// RFID_PACKET_TYPE_COMMAND_ACTIVE
        /// </summary>
        CommandActive = 0x000E,

        // Diagnostic class

        /// <summary>
        /// RFID_PACKET_TYPE_18K6C_INVENTORY_ROUND_BEGIN_DIAGS
        /// </summary>
        InventoryRoundBeginDiagnostics = 0x1004,

        /// <summary>
        /// RFID_PACKET_TYPE_18K6C_INVENTORY_ROUND_END_DIAGS
        /// </summary>
        InventoryRoundEndDiagnostics = 0x1005,

        /// <summary>
        /// RFID_PACKET_TYPE_18K6C_INVENTORY_DIAGS
        /// </summary>
        InventoryDiagnostics = 0x1006,

        /// <summary>
        /// RFID_PACKET_TYPE_INVENTORY_CYCLE_END_DIAGS
        /// </summary>
        InventoryCycleEndDiagnostics = 0x1008,

        /// <summary>
        /// RFID_PACKET_TYPE_SJC_SCANRESULT
        /// </summary>
        SjcScanResult = 0x1009,

        /// <summary>
        /// RFID_PACKET_TYPE_TX_RANDOM_DATA_STATUS
        /// </summary>
        TxRandomDataStatus = 0x100A,

        /// <summary>
        /// RFID_PACKET_TYPE_CSM_PROTSCHED_SM_STATUS
        /// </summary>
        CsmProtocolSchedulerStateMachineStatus = 0x100B,

        /// <summary>
        /// RFID_PACKET_TYPE_CSM_PROTSCHED_LBT_STATUS
        /// </summary>
        CsmProtocolSchedulerLBTStatus = 0x100C,


        // Status class 

        /// <summary>
        /// RFID_PACKET_TYPE_NONCRITICAL_FAULT
        /// </summary>
        NonCriticalFault = 0x2000,

        // Reserved class

        /// <summary>
        /// RFID_PACKET_TYPE_ENGTESTPAT_ZZS
        /// </summary>
        EngineeringTestPatternZzs = 0x3000,

        /// <summary>
        /// RFID_PACKET_TYPE_ENGTESTPAT_FFS
        /// </summary>
        EngineeringTestPatternFfs = 0x3001,

        /// <summary>
        /// RFID_PACKET_TYPE_ENGTESTPAT_W1S
        /// </summary>
        EngineeringTestPatternW1s = 0x3002,

        /// <summary>
        /// RFID_PACKET_TYPE_ENGTESTPAT_W0S
        /// </summary>
        EngineeringTestPatternW0s = 0x3003,

        /// <summary>
        /// RFID_PACKET_TYPE_ENGTESTPAT_BND
        /// </summary>
        EngineeringTestPatternBoundsCheck = 0x3004,

        /// <summary>
        /// RFID_PACKET_TYPE_MBP_READ_REG
        /// </summary>
        MacBypassRead = 0x3005,

        /// <summary>
        /// RFID_PACKET_TYPE_GPIO_READ
        /// </summary>
        GpioRead = 0x3006,

        /// <summary>
        /// RFID_PACKET_TYPE_OEMCFG_READ
        /// </summary>
        OemConfigRead = 0x3007,

        /// <summary>
        /// RFID_PACKET_TYPE_ENG_RSSI
        /// </summary>
        TestRssi = 0x3008,

        /// <summary>
        /// RFID_PACKET_TYPE_ENG_INVSTATS
        /// </summary>
        TestInventoryStatistics = 0x3009,

        /// <summary>
        /// RFID_PACKET_TYPE_ENG_BERTESTRESULT
        /// </summary>
        TestBerPerResult = 0x300A,

        /// <summary>
        /// RFID_PACKET_TYPE_NVMEMUPDCFG
        /// </summary>
        NonVolatileMemoryUpdateConfiguration = 0x300B,

        /// <summary>
        /// RFID_PACKET_TYPE_LPROF_READ_REG
        /// </summary>
        LinkProfileXcvrRegisterRead = 0x300C,

        /// <summary>
        /// RFID_PACKET_TYPE_ENG_XY
        /// </summary>
        TestXyPair = 0x300E,

        // Debug class

        /// <summary>
        /// RFID_PACKET_TYPE_DEBUG
        /// </summary>
        Debug = 0x4000
    }
}
