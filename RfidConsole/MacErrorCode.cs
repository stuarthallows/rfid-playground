// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace RfidConsole
{
    public enum MacErrorCode
    {
        /// <summary>
        /// Command successful with no errors.
        /// </summary>
        MACERR_SUCCESS = 0x0000,

        /// <summary>
        /// This error is set when an invalid command has been issued to the Indy Firmware. The Indy Firmware performs basic bounds checking on command values
        /// </summary>
        CSM_ERR_UNKNOWNCMD = 0x0001,

        /// <summary>
        /// An error occurred during pre-command execution processing. This may happen if the Indy Firmware is unable to transmit a Command-Begin packet to the host.
        /// </summary>
        CSM_ERR_PREEXECPROC = 0x0002,

        /// <summary>
        /// An error occurred during post-command execution processing. This map happen if the Indy Firmware is unable to flush host TX buffers after the main processing of a given command is complete
        /// </summary>
        CSM_ERR_POSTEXECPROC = 0x0003,

        /// <summary>
        /// This is set when an unsupported ENGTEST sub-command has been indicated via the HST_ENGTST_ARG0 register, bits 7:0. 
        /// </summary>
        CSM_ERR_BADENGTESTSUBCMD = 0x0004,

        /// <summary>
        /// Set if an invalid / unsupported UHF RFID Transceiver register is detected in the HST_MBP_ADDR after an MBPRDREG command is issued to the Indy Firmware.
        /// </summary>
        CSM_ERR_MBPRDADDR = 0x0005,

        /// <summary>
        /// Set if an invalid / unsupported register is detected in the HST_MBP_ADDR after an MBPWRREG command is issued to the Indy Firmware.
        /// </summary>
        CSM_ERR_MBPWRADDR = 0x0006,

        /// <summary>
        /// Set if the CPU module fails to initialize on Indy Firmware boot
        /// </summary>
        CSM_ERR_SUBSYSINIT_CPU = 0x0007,

        /// <summary>
        /// Set if the Debug module fails to initialize on Indy Firmware boot.
        /// </summary>
        CSM_ERR_SUBSYSINIT_DBG = 0x0008,

        /// <summary>
        /// Set if the Core State Machine fails to initialize on Indy Firmware boot
        /// </summary>
        CSM_ERR_SUBSYSINIT_CSM = 0x0009,

        /// <summary>
        /// Set if the OEM configuration module fails to initialize on Indy Firmware boot.
        /// </summary>
        CSM_ERR_SUBSYSINIT_OEMCFG = 0x000A,

        /// <summary>
        /// Set of the HOST interface module fails to initialize on Indy Firmware boot.
        /// </summary>
        CSM_ERR_SUBSYSINIT_HOSTIF = 0x000B,

        /// <summary>
        /// Set if the UHF RFID Transceiver low level interface module fails to initialize on Indy Firmware boot
        /// </summary>
        CSM_ERR_SUBSYSINIT_TILIF = 0x000C,

        /// <summary>
        /// Set if the BIST module fails to initialize on Indy Firmware boot
        /// </summary>
        CSM_ERR_SUBSYSINIT_BIST = 0x000D,

        /// <summary>
        /// Set if the GPIO module fails to initialize on Indy Firmware boot.
        /// </summary>
        CSM_ERR_SUBSYSINIT_GPIO = 0x000F,

        /// <summary>
        /// Set of the RF Transceiver Control module fails to initialize on Indy Firmware boot
        /// </summary>
        CSM_ERR_SUBSYSINIT_RFTC = 0x0010,

        /// <summary>
        /// Set if the RFID Protocol module(s) fail to initialize on Indy Firmware boot
        /// </summary>
        CSM_ERR_SUBSYSINIT_PROT = 0x0011,

        /// <summary>
        /// Set if the RFID protocol scheduler module detects an unknown state - likely indicates firmware corruption or runtime SRAM corruption by errant code
        /// </summary>
        CSM_ERR_PROTSCHED_UNKST = 0x0012,

        /// <summary>
        /// Set if the Antenna configuration dwell time and inventory round count are both zero - which is illegal and ambiguous
        /// </summary>
        CSM_ERR_PROTSCHED_AMBANT = 0x0013,

        /// <summary>
        /// Set if the protocol scheduler detects that no logical antennas have been enabled using the HST_ANT_DESC_CFG register bank.
        /// </summary>
        CSM_ERR_PROTSCHED_NODESC = 0x0014,

        /// <summary>
        /// Set when a bogus physical antenna port definition value is used - this likely means that the TX and RX port values are not the same - which is required for Indy Development Platform
        /// </summary>
        CSM_ERR_PROTSCHED_PORTDEF = 0x0015,

        /// <summary>
        /// Set by the protocol scheduler when no frequency channels have been enabled.
        /// </summary>
        CSM_ERR_PROTSCHED_NOFRQCH = 0x0016,

        /// <summary>
        /// Set by the protocol scheduler when a bogus regulatory region has been detected in HST_REGULATORY_REGION
        /// </summary>
        CSM_ERR_PROTSCHED_BADREGION = 0x0017,

        /// <summary>
        /// Set by the protocol schedulers FCC state machine when a bogus FCC frequency hop value has been written to HST_PROTSCH_FTIME, Bank 0 - only 100,200,400 milliseconds are valid values.
        /// </summary>
        CSM_ERR_PROTSCHED_BADFTIME = 0x0018,

        /// <summary>
        /// Not currently set by firmware
        /// </summary>
        CSM_ERR_PROTSCHED_FTUNETO = 0x0019,

        /// <summary>
        /// Set if the OEM hardware-option configuration module fails to initialize on Indy Firmware boot
        /// </summary>
        CSM_ERR_SUBSYSINIT_OEMHWOPTS = 0x001A,

        /// <summary>
        /// Set if the firmware failed to initialize the NV Memory Update module at boot time
        /// </summary>
        CSM_ERR_SUBSYSINIT_NVMEMUPD = 0x001B,

        /// <summary>
        /// Set if the firmware CPU module's reset device logic is called with a bogus key. This will generally only happen if the system has experienced a crash and this logic is being 
        /// called through an invalid call chain - likely due to some sort of corruption
        /// </summary>
        CSM_ERR_BAD_RESET_KEY = 0x001C,

        /// <summary>
        /// Set if the device reset logic fails to actually reset the device - likely due to a MCU related hardware failure or system corruption
        /// </summary>
        CSM_ERR_DEV_RESET_FAILED = 0x001D,

        /// <summary>
        /// Set *prior* to entering non volatile memory update mode if the current global Indy Firmware error status is indicating an error. The MAC will not enter non volatile memory 
        /// update mode if there is currently an error. The host should use the CLRERR command to clear any errors; if this doesn't work the device may need to be manually updated 
        /// using the recovery method indicated in the Indy Firmware Datasheet.
        /// </summary>
        CSM_ERR_NVMEMUPD_ABORT_MACERRNO = 0x001E,

        /// <summary>
        /// Set if an internal memory bounds check fails while in Non volatile memory update mode. If these errors occur the Indy Firmware tries very hard not to update non-volatile 
        /// memory with bogus data. This error occurs likely due to a system corruption.
        /// </summary>
        CSM_ERR_NVMEMUPD_INT_MEMBNDS = 0x001F,

        /// <summary>
        /// Set if the non volatile memory mode entry logic detects an invalid key. This would occur if the calling logic erroneously called the non-volatile memory logic due to system 
        /// corruption / firmware error.
        /// </summary>
        CSM_ERR_NVMEMUPD_ENTRYKEY = 0x0020,

        /// <summary>
        /// Set if, during non volatile memory update mode the firmware fails to write flash at the lowest level. This is likely due to flash lock bits being set (i.e. via tools like SAM-BA) 
        /// or a system corruption.
        /// </summary>
        CSM_ERR_NVMEMUPD_NVFLUSH = 0x0021,

        /// <summary>
        /// Set if write verification logic fails after writing data at the lowest level to flash. This may indicate problems with MCU device flash hardware. This can occur if the MCU 
        /// device flash has been updated too many times
        /// </summary>
        CSM_ERR_NVMEMUPD_WRVERFAIL = 0x0022,

        /// <summary>
        /// Set by the protocol scheduler if the HST_RFTC_FRQCH_CMDSTART register has been set to an invalid channel
        /// </summary>
        CSM_ERR_INVAL_START_CHAN = 0x0023,

        /// <summary>
        /// Set by the protocol scheduler if an invalid protocol algorithm has been selected via the HST_INV_CFG register.
        /// </summary>
        CSM_ERR_PROTSCHED_UNK_ALGO = 0x0024,

        /// <summary>
        /// Set by the core state machine if an invalid power management mode has been specified in the HST_PWRMGMT register
        /// </summary>
        CSM_ERR_INVAL_PWRMODE = 0x0025,

        /// <summary>
        /// This is set if a system corruption has occurred and the logic is unable to determine the desired power management mode.
        /// </summary>
        CSM_ERR_PWRMODE_CORRUPT = 0x0026,

        /// <summary>
        /// Set if the non volatile memory mode logic fails to transmit a packet to the host during non volatile memory update
        /// </summary>
        CSM_ERR_NVMEMUPD_TXFAIL = 0x0027,

        /// <summary>
        /// Set during non volatile memory update if the range indicated for updates falls outside the valid non-volatile memory ranges available on the device
        /// </summary>
        CSM_ERR_NVMEMUPD_UPD_BOUNDS = 0x0028,

        /// <summary>
        /// An unknown error has occurred during non-volatile memory updates - likely a system corruption.
        /// </summary>
        CSM_ERR_NVMEMUPD_UNKNOWN = 0x0029,

        /// <summary>
        /// Set during non volatile memory mode if the firmware does not receive a packet from the host within 60 seconds. This may occur if the host side application has crashed or 
        /// the physical interface has been removed or corrupted.
        /// </summary>
        CSM_ERR_NVMEMUPD_RXTO = 0x002A,

        /// <summary>
        /// This error code is generated when the host / user attempts to use a GPIO pin that has previously been configured as unavailable in the OEM Configuration Area entry GPIO_AVAIL
        /// </summary>
        CSM_ERR_GPIO_NOTAVAIL = 0x002B,

        /// <summary>
        /// This error code is generated when the host / user attempts to use an antenna pin that has previously been configured as unavailable in the OEM Configuration Area entry ANT_AVAIL
        /// </summary>
        CSM_ERR_ANT_NOTAVAIL = 0x002C,

        /// <summary>
        /// Set by the command processor when a command is invoked from the host, which has been defined, but is not available in the Indy Firmware codebase. This situation can occur if, 
        /// for instance, a command is disabled by means of a compile-time switch.
        /// </summary>
        CSM_ERR_CMDNOTAVAILABLE = 0x002D,

        /// <summary>
        /// Set by the protocol scheduler when no CORDIC values are found in the OEM configuration area. CORDIC values are part of the LBT configuration. See the OEM Configuration 
        /// Section of the Firmware Datasheet for more details on these settings. Cordic configuration values are only required when LBT is enabled.
        /// </summary>
        CSM_ERR_NOCORDICDEF = 0x002E,

        /// <summary>
        /// Set if the firmware failed to initialize the Debug subsystem at boot time.
        /// </summary>
        CSM_ERR_SUBSYSINIT_DEBUG = 0x002F,

        /// <summary>
        /// Set if the firmware failed to initialize the Trace subsystem at boot time.
        /// </summary>
        CSM_ERR_SUBSYSINIT_TRACE = 0x0030,

        /// <summary>
        /// Set if the firmware failed the Target Build and Physical Device Check at boot time.
        /// </summary>
        CSM_ERR_BUILD_TARGET_DEVICE_MISMATCH = 0x0031,

        /// <summary>
        /// Set if the firmware failed to properly set MAC Error diagnostic codes. Actual MAC Error may not correctly be reflected by the MAC Error register.
        /// </summary>
        CSM_ERR_DIAGNOSTICS = 0x0032,

        /// <summary>
        /// Set if the MAC register default value initialization module fails to initialize on Indy Firmware boot.
        /// </summary>
        CSM_ERR_SUBSYSINIT_HOSTIFREGS_INIT = 0x0033,

        /// <summary>
        /// Set if the firmware failed to initialize the Handshake interface subsystem at boot time.
        /// </summary>
        CSM_ERR_SUBSYSINIT_HANDSHAKE = 0x0034,

        /// <summary>
        /// Set if the HST_NV_UPDATE_CONTROL MAC register had an invalid update_mode set.
        /// </summary>
        CSM_ERR_NVMEMUPD_INVALID_MODE = 0x0035,

        /// <summary>
        /// Set if a Gen2 command is attempted following a critical error during system initialization. Typically caused by a failed OEM read attempt and can usually be resolved by formatting OEM.
        /// </summary>
        CSM_ERR_INVALID_CMD_WHILE_IN_CRIT_ERROR = 0x0036,

        /// <summary>
        /// Set if an unknown critical error is detected at the end of system initialization. Typically caused by a failed OEM read attempt and can usually be resolved by formatting OEM.
        /// </summary>
        CSM_ERR_CRITICAL_ERROR_UNKNOWN = 0x0037,

        /// <summary>
        /// RESERVED
        /// </summary>
        RESERVED_0x0101 = 0x0101,

        /// <summary>
        /// Set by the USB interface module when an unsupported descriptor TYPE has been requested by the host (i.e. not a device, string, configuration descriptor type. This may be due to 
        /// compatibility problems with the USB host.
        /// </summary>
        HOSTIF_ERR_USBDESC = 0x0102,

        /// <summary>
        /// Set by the USB interface module when an unsupported device descriptor index has been requested by the Host.
        /// </summary>
        HOSTIF_ERR_USBDESCIDX = 0x0103,

        /// <summary>
        /// Set by the USB interface module when it is unable to transmit the response to a request on USB endpoint 0 (aka control endpoint). This may be due to compatibility or synchronization 
        /// problems with the USB host.
        /// </summary>
        HOSTIF_ERR_USBTXEP0 = 0x0104,

        /// <summary>
        /// RESERVED
        /// </summary>
        RESERVED_0x0105 = 0x0105,

        /// <summary>
        /// Set by the USB interface module when higher level firmware requests an unsupported buffer length. This may be due to a firmware build error or corrupted firmware in flash.
        /// </summary>
        HOSTIF_ERR_USBRXBUFFSZ = 0x0106,

        /// <summary>
        /// This is set by the Host interface module when the underlying physical interface module returns an unknown error code on receive from the host. This may be due to a firmware build 
        /// issue, corrupted firmware image or corrupted SRAM due to errant Indy Firmware code.
        /// </summary>
        HOSTIF_ERR_RXUNKNOWN = 0x0107,

        /// <summary>
        /// This is set by the Host interface module when the underlying physical interface module returns an unknown error code on transmit to the Host. This may be due to a firmware build 
        /// issue, corrupted firmware image or corrupted SRAM due to errant code.
        /// </summary>
        HOSTIF_ERR_TXUNKNOWN = 0x0108,

        /// <summary>
        /// This is set when the Host interface code detects that its internal state machine out of sync. This could be due to a corrupted firmware image or corrupted SRAM due to errant Indy Firmware code.
        /// </summary>
        HOSTIF_ERR_BADIFSTATE = 0x0109,

        /// <summary>
        /// RESERVED
        /// </summary>
        RESERVED_0x010A = 0x010A,

        /// <summary>
        /// Set by the host interface module when an invalid Indy Firmware register read or write is attempted (either by the host or internally by the Indy Firmware).
        /// </summary>
        HOSTIF_ERR_REGADDR = 0x010B,

        /// <summary>
        /// RESERVED
        /// </summary>
        RESERVED_0x010C = 0x010C,

        /// <summary>
        /// This is set by the host interface module during initialization if it is unable to retrieve USB descriptors from non-volatile memory (i.e. flash) OEM configuration area. This may be 
        /// due to a corrupt or unformatted OEM Configuration area. It may also be due to a firmware build issue if the OEM configuration definition is out of sync with the Indy Firmware code.
        /// </summary>
        HOSTIF_ERR_USBDESCINIT = 0x010D,

        /// <summary>
        /// This is set when the host attempts to *write* a value to a selector type register that is out of range for that selector.
        /// </summary>
        HOSTIF_ERR_SELECTORBNDS = 0x010E,

        /// <summary>
        /// RESERVED
        /// </summary>
        RESERVED_0x010F = 0x010F,

        /// <summary>
        /// Not currently set by Indy Firmware
        /// </summary>
        HOSTIF_ERR_PKTALIGN = 0x0110,

        /// <summary>
        /// Set by the low level host interface logic if an upper level requests an unsupported raw mode. This may occur if the system is corrupted.
        /// </summary>
        HOSTIF_ERR_BADRAWMODE = 0x0111,

        /// <summary>
        /// Set by the low level host interface logic if a system corrupt occurs and the link manager can not determine the current link state.
        /// </summary>
        HOSTIF_ERR_UNKLNKSTATE = 0x0112,

        /// <summary>
        /// Set by the low level host interface logic if an unknown / unsupported control command is received from the host. This may occur if the host logic and the Indy Firmware logic are 
        /// out of sync in terms of the lowest level host interface (UART, USB).
        /// </summary>
        HOSTIF_ERR_UNKUSBSETUP = 0x0113,

        /// <summary>
        /// This is set if the upper layer host logic attempts to receive data and the lower layer cannot support the buffer size requested. This will happen if the system is corrupted.
        /// </summary>
        HOSTIF_ERR_UARTRXBUFFSZ = 0x0114,

        /// <summary>
        /// Set by the low level host interface logic if a control command is received from the host while in raw mode - which is not allowed. This would happen if the host caused the Indy 
        /// Firmware to enter non-volatile memory update mode, which uses raw mode, and then the host proceeded to issue control commands
        /// </summary>
        HOSTIF_ERR_RAWMODECTL = 0x0115,

        /// <summary>
        /// Set by the host interface module at boot time if the OEM configuration area is specifying an unsupported host interface.
        /// </summary>
        HOSTIF_ERR_UNKHOSTIF = 0x0116,

        /// <summary>
        /// Set by the host interface module at boot time if the OEM configuration area is specifying an unsupported regulatory standard.
        /// </summary>
        HOSTIF_ERR_UNKREGSTD = 0x0117,

        /// <summary>
        /// Set by host interface module if Debug Id is invalid.
        /// </summary>
        HOSTIF_ERR_DEBUGID = 0x0118,

        /// <summary>
        /// Set by host interface module if Debug Buffer overflows.
        /// </summary>
        HOSTIF_ERR_DEBUGOVERFLOW = 0x0119,

        /// <summary>
        /// Set by the host interface module when a Read-Only Indy Firmware register write is attempted by the host
        /// </summary>
        HOSTIF_ERR_REGREADONLY = 0x011A,

        /// <summary>
        /// Set by the host interface module when an Write Only Indy Firmware register read is attempted by host.
        /// </summary>
        HOSTIF_ERR_REGWRITEONLY = 0x011B,

        /// <summary>
        /// Set by the host interface module if the default region dependent parameters are invalid.
        /// </summary>
        HOSTIF_ERR_BADREGIONINITVALUES = 0x011C,

        /// <summary>
        /// Set by an ENGTEST sub-command with an invalid argument.  Diagnostics: EngTest Sub Commnad, EngTest Arg0, EngTest Arg1
        /// </summary>
        HOSTIF_ERR_INVALIDENGTESTARG = 0x011D,

        /// <summary>
        /// Set by Set Frequency Command with an invalid argument. When this error is set, the result registers will be set to 0xFFFFFFFF.
        /// </summary>
        HOSTIF_ERR_INVALIDSETFREQARG = 0x011E,

        /// <summary>
        /// Set when an invalid Inventory RSSI Filtering configuration has been configured
        /// </summary>
        HOSTIF_ERR_INVALID_RSSI_FILTERING = 0x011F,

        /// <summary>
        /// Set when an invalid HST_TAGACC_CNT value is specified.
        /// </summary>
        HOSTIF_ERR_INVALID_TAGACC_CNT = 0x0120,

        /// <summary>
        /// Set when an invalid BlockWrite mode is specified in HST_IMPINJ_EXTENSIONS
        /// </summary>
        HOSTIF_ERR_INVALID_BW_MODE = 0x0121,

        /// <summary>
        /// Set when an invalid MAC Register Initialization pair (Control/Data) is found during the MAC Register initialization.  Diagnostics: Id, Control/Data Pair Index, Control Word OEM Address
        /// </summary>
        HOSTIF_ERR_OEM_MAC_REG_INIT_CTRL_ERROR = 0x0122,

        /// <summary>
        /// Set when an invalid MAC Register Initialization write occurs found during the MAC Register initialization.  Diagnostics: Original Mac Error, Control/Data Pair Index, Control Word OEM Address
        /// </summary>
        HOSTIF_ERR_OEM_MAC_REG_INIT_WRITE_ERROR = 0x0123,

        /// <summary>
        /// Set by protocol if truncation is set in the Select configuration register, since truncation is unsupported.
        /// </summary>
        PROTOCOL_ERR_TRUNCATION_UNSUPPORTED = 0x0200,

        /// <summary>
        /// This is set during the PLL lock logic when a bounds check fails while checking the frequency channel configuration registers.
        /// </summary>
        RFTC_ERR_BADFRQCHAN = 0x0300,

        /// <summary>
        /// This is set if an unsupported frequency hopping mode is detected - during the PLL lock logic.
        /// </summary>
        RFTC_ERR_BADHOPMODE = 0x0301,

        /// <summary>
        /// This is set if the PLL fails to lock.
        /// </summary>
        RFTC_ERR_PLLFAILEDTOLOCK = 0x0302,

        /// <summary>
        /// This is set when the RFTC module's AUX ADC function times out waiting for an ADC conversion.
        /// </summary>
        RFTC_ERR_XCVRADC_TIMEDOUT = 0x0303,

        /// <summary>
        /// This is set when the RFTC module times out waiting for UHF RFID Transceiver to indicate RX or TX filter tuning is complete.
        /// </summary>
        RFTC_ERR_FILTTUNE_TIMEOUT = 0x0304,

        /// <summary>
        /// This is set when the RFTC module detects ambient temperature sensor indicates too hot.
        /// </summary>
        RFTC_ERR_AMBIENTTEMPTOOHOT = 0x0305,

        /// <summary>
        /// This is set when the RFTC module detects transceiver temperature sensor indicates too hot.
        /// </summary>
        RFTC_ERR_XCVRTEMPTOOHOT = 0x0306,

        /// <summary>
        /// This is set when the RFTC module detects PA temperature sensor indicates too hot.
        /// </summary>
        RFTC_ERR_PATEMPTOOHOT = 0x0307,

        /// <summary>
        /// This is set when the RFTC module detects that the delta between the PA temperature and the ambient temperature is too great.
        /// </summary>
        RFTC_ERR_PADELTATEMPTOOBIG = 0x0308,

        /// <summary> 
        /// This is set when the reverse power level is too high as measured by the configured reverse power level threshold in the register set.  Diagnostics: Forward Power, Reverse Power, Threshold
        /// </summary>
        RFTC_ERR_REVPWRLEVTOOHIGH = 0x0309,

        /// <summary>
        /// This is set when an incorrect current gain setting is passed into the IFLNA gain adjustment logic. May indicate corrupted code.
        /// </summary>
        RFTC_ERR_BADIFLNAGAIN = 0x030A,

        /// <summary>
        /// Returned by RFTC code when errors occur in transmitting a bit over the RF interface
        /// </summary>
        RFTC_ERR_TXRF_BIT_FAILED = 0x030B,

        /// <summary>
        /// Returned by RFTC code when errors occur in transmitting a buffer of bytes over the RF interface
        /// </summary>
        RFTC_ERR_TXRF_BYTE_FAILED = 0x030C,

        /// <summary>
        /// Returned by RFTC code when errors occur in transmitting an "end of transfer" command over the RF interface
        /// </summary>
        RFTC_ERR_TXRF_EOT_FAILED = 0x030D,

        /// <summary>
        /// Returned by RFTC code when errors occur in transmitting a "preamble" command over the RF interface
        /// </summary>
        RFTC_ERR_TXRF_PREAM_FAILED = 0x030E,

        /// <summary>
        /// Returned by RFTC code when errors occur in transmitting a "frame-sync" command over the RF interface
        /// </summary>
        RFTC_ERR_TXRF_FSYNC_FAILED = 0x030F,

        /// <summary>
        /// Indicates that the RF Transceiver failed to set expected ISR bits in a timely fashion. Indicates a failure in either the RFTC state machine logic or in the RF Transceiver state machine logic.
        /// </summary>
        RFTC_ERR_RXRF_ISR_TIMEOUT = 0x0310,

        /// <summary>
        /// This is set when invalid link parameters are detected when the filter tuning logic is run.
        /// </summary>
        RFTC_ERR_INVALIDLINKPARMS = 0x0311,

        /// <summary>
        /// This indicates a failure in either the RFTC state machine logic or in the RF Transceiver state machine logic. This error can only occur if the RF Transceiver starts filling its 
        /// RX FIFO with received data, but fails return the requested number of bits in a timely fashion.
        /// </summary>
        RFTC_ERR_RXRF_INTERPKTTIMEOUT = 0x0312,

        /// <summary>
        /// Not currently in use. May occur in the future when switching between link profiles if some of the required information is not properly coded in the Indy Firmware.
        /// </summary>
        RFTC_ERR_NO_LINKPROFHDR = 0x0313,

        /// <summary>
        /// This error occurs if the RF Transceiver is being loaded with an invalid profile
        /// </summary>
        RFTC_ERR_PROFILE_INVALID = 0x0314,

        /// <summary>
        /// Internal error. The error is the direct result of the Indy Firmware having to do a "dBm to linear" conversion on a dBm measurement that is outside the range of -99dBm 
        /// through +45dBm. It the unlikely event that this error is encountered, it is probably the result of a faulty RF Peak Detector, a bug in the code that computes the 
        /// dBm value from the RF Peak Detector ADC reading, or a faulty external PA circuit.
        /// </summary>
        RFTC_ERR_DBMVALOUTOFRANGE = 0x0315,

        /// <summary>
        /// If, during RF power-ramping, it is determined that the RF power at the antenna port has momentarily exceeded 35dBm, or has exceeded 33dBm steady-state, this error will be 
        /// thrown. Encountering this error is often the result attempting to transmit on an open antenna port or in other cases an incorrect calibration of the gross gains. Make 
        /// sure an antenna is connected on the physical port in use or see Indy Firmware command 0x1B for more information on how to calibrate the system.
        /// </summary>
        RFTC_ERR_FWDPWRLEVTOOHIGH = 0x0316,

        /// <summary>
        /// Internal error that may occur if memory is corrupted.
        /// </summary>
        RFTC_ERR_NO_GROSSPWRENTRY = 0x0317,

        /// <summary>
        /// Indicates that the target power (in Indy Firmware Virtual Register 0x706) is higher than the maximum allowed output power, which is +33dBm.
        /// </summary>
        RFTC_ERR_TARGETPWRTOOHIGH = 0x0318,

        /// <summary>
        /// RESERVED. Deprecated RFTC_ERR_REFVOLT_OUTOFBOUNDS
        /// </summary>
        RESERVED_0x0318 = 0x0319,

        /// <summary>
        /// Indicates that the measured value of the antenna-sense resistor (reported in the Indy Firmware Virtual Register 0x703) exceeds the threshold specified (specified in the 
        /// Indy Firmware Virtual register 0xB12). To determine which antenna was disconnected, the list of enabled antennas will need to be scanned for the one exceeding the 
        /// threshold (this is done by iterating through all valid selectors in register 0x701 and examining the MAC_ANT_DESC_STAT register at address 0x703.
        /// </summary>
        RFTC_ERR_ANTENNADISCONNECTED = 0x031A,

        /// <summary>
        /// Indicates that the OEMCFG's HW_OPTIONS_FORMAT value is not recognized by the RFTC subsystem
        /// </summary>
        RFTC_ERR_UNREC_HWOPTFORMAT = 0x031B,

        /// <summary>
        /// Indicates that the forward power detection option found in OEMCFG's HW_OPTIONS0 field is not recognized by the RFTC subsystem
        /// </summary>
        RFTC_ERR_HWOPT_BADFWDPWROPT = 0x031C,

        /// <summary>
        /// Indicates that the reverse power detection option found in OEMCFG's HW_OPTIONS0 field is not recognized by the RFTC subsystem
        /// </summary>
        RFTC_ERR_HWOPT_BADREVPWROPT = 0x031D,

        /// <summary>
        /// Indicates that the DRM Filter option found in OEMCFG's HW_OPTIONS0 field is not recognized by the RFTC subsystem
        /// </summary>
        RFTC_ERR_HWOPT_BADDRMFILTOPT = 0x031E,

        /// <summary>
        /// Indicates that ambient temperature sensor option found in OEMCFG's HW_OPTIONS0 field is not recognized by the RFTC subsystem
        /// </summary>
        RFTC_ERR_HWOPT_BADAMBTEMPOPT = 0x031F,

        /// <summary>
        /// Indicates that PA temperature sensor option found in OEMCFG's HW_OPTIONS0 field is not recognized by the RFTC subsystem
        /// </summary>
        RFTC_ERR_HWOPT_BADPATEMPOPT = 0x0320,

        /// <summary>
        /// Indicates that transceiver temperature sensor option found in OEMCFG's HW_OPTIONS0 field is not recognized by the RFTC subsystem
        /// </summary>
        RFTC_ERR_HWOPT_BADXCVRTEMPOPT = 0x0321,

        /// <summary>
        /// Indicates that antenna-sense resistor sensor option found in OEMCFG's HW_OPTIONS0 field is not recognized by the RFTC subsystem
        /// </summary>
        RFTC_ERR_HWOPT_BADANTSENSOPT = 0x0322,

        /// <summary>
        /// The range specified for the IF LNA AGC gain limits is bad. Either the "min" is higher than the "max", or the min or max setting is incorrect.
        /// </summary>
        RFTC_ERR_BADIFLNAAGCRANGE = 0x0323,

        /// <summary>
        /// When invoking the CMD_LPROF_RDXCVRREG or CMD_LPROF_WRXCVRREG commands, one of the arguments is the selector of a valid link profile. New link profile selectors cannot be 
        /// created through these commands, so if a selector outside this range is passed, the RFTC_ERR_LPROFBADSELECTOR error will be generated.
        /// </summary>
        RFTC_ERR_LPROFBADSELECTOR = 0x0324,

        /// <summary>
        /// One of the arguments to the CMD_LPROF_RDXCVRREG or CMD_LPROF_WRXCVRREG commands is the RF transceiver register address to configure. If the address passed is not a valid 
        /// transceiver address, this error will be thrown. This error is also generated if an invalid transceiver address is detected in an OEM custom profile.
        /// </summary>
        RFTC_ERR_BADXCVRADDR = 0x0325,

        /// <summary>
        /// Not all valid transceiver addresses may be configured through the link profiles. The excluded addresses include those registers which are read-only (refer to the 
        /// transceiver register map), and the indirect address for the R2T command register: 0x0105.
        /// </summary>
        RFTC_ERR_XCVRADDRNOTINLIST = 0x0326,

        /// <summary>
        /// Set by the RFTC module if an unsupported RFLNA gain level is requested
        /// </summary>
        RFTC_ERR_BAD_RFLNA_GAIN_REQ = 0x0327,

        /// <summary>
        /// Set by the RFTC module if an unsupported IFLNA gain level is requested
        /// </summary>
        RFTC_ERR_BAD_IFLNA_GAIN_REQ = 0x0328,

        /// <summary>
        /// Set by the RFTC module if an unsupported AGC/MIXER gain level is requested.
        /// </summary>
        RFTC_ERR_BAD_AGCMIX_GAIN_REQ = 0x0329,

        /// <summary>
        /// Set by the RFTC module if an unsupported compensation option is detected at OEMCFG address 0xA1.
        /// </summary>
        RFTC_ERR_HWOPT_BADFWDPWRCOMPOPT = 0x032A,

        /// <summary>
        /// This error is generated if the PLL Divider Value is zero.
        /// </summary>
        RFTC_ERR_INVALID_PLL_DIVIDER_VALUE = 0x032B,

        /// <summary>
        /// This error is generated if the external LO signal level is below the threshold specified in register HST_RFTC_SJC_EXTERNALLOTHRSH.
        /// </summary>
        RFTC_ERR_SJC_EXTERNALLOTOOLOW = 0x032C,

        /// <summary>
        /// This error is generated if SJC is enabled, and the LO source is not external
        /// </summary>
        RFTC_ERR_SJC_EXTERNALLONOTSELECTED = 0x032D,

        /// <summary>
        /// This error is generated if the LO source is incorrectly defined in the OEM Config registers.
        /// </summary>
        RFTC_ERR_BADLOSOURCE = 0x032E,

        /// <summary>
        /// This error is generated if there is a general error in the Random Data Transmit function.  Diagnostics: Id, Value 1, Value 2
        /// </summary>
        RFTC_ERR_GENERALRANDOMDATA = 0x032F,

        /// <summary>
        /// This error is generated if there is Transceiver Health check failure and the handler is set to enable Mac Error. See OEM Config XCVR_HEALTH_CHECK_CFG.
        /// </summary>
        RFTC_ERR_XVCR_HEALTH_CHECK_FAIL = 0x0330,

        /// <summary>
        /// This error is generated if the OEM custom profile header is invalid.
        /// </summary>
        RFTC_ERR_INVALID_OEM_PROFILE_HEADER = 0x0331,

        /// <summary>
        /// This error is generated if an error during the Auto Read of the Rx FIFO Read is detected.  Diagnostics: Id, Value 1, Value 2
        /// </summary>
        RFTC_ERR_AUTO_READ_RX_FIFO = 0x0332,

        /// <summary>
        /// This error is general error generated if an error occurs during the DC Offset Calibration.  Diagnostics: Min Index, Min Index Sum, Min Index Count
        /// </summary>
        RFTC_ERR_DC_OFFSET_CALIBRATION = 0x0333,

        /// <summary>
        /// This error is general error generated if an error occurs during the LBT RSSI Calibration. If noise floor versus calibration value do not have a significant difference 
        /// this error will occur. User should check the injected reference signal for level and frequency.
        /// </summary>
        RFTC_ERR_LBT_RSSI_CALIBRATION = 0x0334,

        /// <summary>
        /// This error is related to a PA Bias Calibration Configuration error.  Diagnostics: Measurement Configuration, Current Per LSB, Target Current
        /// </summary>
        RFTC_ERR_PA_BIAS_CAL_CONFIG = 0x0335,

        /// <summary>
        /// This error is generated when the requested forward power level is not achieved during power ramp.  See HST_ANT_DESC_RFPOWER for the power level requested, MAC_RFTC_PAPWRLEV 
        /// for the power level achieved, and HST_RFTC_FWDPWRTHRSH for the error threshold.  Diagnostics: Requested Power, Actual Power, Threshold
        /// </summary>
        RFTC_ERR_FWDPWRLEVERROR = 0x0336,

        /// <summary>
        /// Indicates that PA Bias DAC Control option found in OEMCFG's HW_OPTIONS2 field is not recognized by the RFTC subsystem.
        /// </summary>
        RFTC_ERR_HWOPT_BADPABIASDACCTL = 0x0337,

        /// <summary>
        /// This error is related to a PA Bias Calibration measurement variation error.  Diagnostics: DAC Value, Measurement Delta, Measurment Delta Value
        /// </summary>
        RFTC_ERR_PA_BIAS_CAL_MEASUREMENT = 0x0338,

        /// <summary>
        /// This error is related to a PA Bias Calibration when the target current is not found.  Diagnostics: Last DAC Value, Last Measurement, Current Value
        /// </summary>
        RFTC_ERR_PA_BIAS_CAL_NOT_FOUND = 0x0339,

        /// <summary>
        /// This error is generated when the Gross Gain Config Value in the OEM is invalid. Min index must be less than Max, and Max must be less than the absolute max of 32.  
        /// Diagnostics: Min Index, Max Index, Config
        /// </summary>
        RFTC_ERR_GROSSGAIN_CONFIG_INVALID = 0x033A,

        /// <summary>
        /// This error is generated if SJC is enabled with an R500 device.  Diagnostics: Function Id, 0, 0
        /// </summary>
        RFTC_ERR_SJC_NOT_AVAILABLE_R500 = 0x033B,

        /// <summary>
        /// This error is general error generated if an error occurs during the Gross Gain Calibration. User should check the gross gain calibration configuration.  
        /// Diagnostics: Min Index, Max Index, Power Scalar
        /// </summary>
        RFTC_ERR_GROSSGAIN_CALIBRATION = 0x033C,

        /// <summary>
        /// This is set by the CPU support module when an attempt is make to read IO lines not configured for input. This may be due to internal firmware error or the host having 
        /// incorrectly configured the Indy Development Platform GPIO lines.
        /// </summary>
        IO_INVAL_RDMASK = 0x0401,

        /// <summary>
        /// This is set by the CPU support module when an attempt is make to write IO lines not configured for output. This may be due to internal firmware error or the host having 
        /// incorrectly configured the Indy Development Platform GPIO lines.
        /// </summary>
        IO_INVAL_WRMASK = 0x0402,

        /// <summary>
        /// This is set by the CPU module when a bounds check fails when accessing non-volatile memory - the caller has passed an incorrect RAM address. This is likely due to 
        /// errant Indy Firmware code.
        /// </summary>
        IO_INVAL_PTR_RAM = 0x0403,

        /// <summary>
        /// This is set by the CPU module when a bounds check fails when attempting to read or write to non-volatile memory. This is likely due to errant Indy Firmware code.
        /// </summary>
        IO_INVAL_PTR_NV = 0x0404,

        /// <summary>
        /// This is set by the CPU module when a bounds check fails when attempting to read or write to non-volatile memory. This is likely due to errant Indy Firmware code.
        /// </summary>
        IO_INVAL_PTR_NV_ALIGN = 0x0405,

        /// <summary>
        /// This is set by the CPU module while attempting to write to non-volatile memory (i.e. flash). This is a flash lock error and may be due to corrupted image or misconfigured 
        /// firmware or hardware problems. If this error is detected by the host, it may which to attempt to read the devices OEM configuration area and save it on the host in order 
        /// to preserve device specific settings.
        /// </summary>
        IO_NV_LOCK_ERR = 0x0406,

        /// <summary>
        /// This is set by the CPU module while attempting to write to non-volatile memory (i.e. flash). This is a low-level flash write error and may be due to misconfigured firmware 
        /// image, timing problems stemming from board hardware failures, or because the flash has exceeded is limitations for writes. . If this error is detected by the host, it may 
        /// which to attempt to read the devices OEM configuration area and save it on the host in order to preserve device specific settings.
        /// </summary>
        IO_NV_PROG_ERR = 0x0407,

        /// <summary>
        /// This is set by the OEM Configuration module when an OEM configuration Address bounds check fails when accessing the OEM configuration space. This may be due to errant 
        /// Indy Firmware code or errant Host code.
        /// </summary>
        IO_OEMCFG_ADDR_BOUNDS = 0x0408,

        /// <summary>
        /// This is set by the OEM Configuration module when a non-volatile memory bounds check fails when accessing the OEM configuration space. This may be due to errant Indy 
        /// Firmware code or errant Host code.
        /// </summary>
        IO_OEMCFG_NV_BOUNDS = 0x0409,

        /// <summary>
        /// This is set by the OEM Configuration module's format facility used as the code calling it fails to pass in the correct "format key" argument. This is a failsafe to 
        /// prevent errant code from inadvertently reformatting flash - due to an invalid branch instruction etc. This will occur when errant code jumps to the format facility 
        /// incorrectly.
        /// </summary>
        IO_OEMCFG_FMT_KEY = 0x040A,

        /// <summary>
        /// This is set by the OEM Configuration module when it fails to flush in memory buffers to non-volatile memory. This may be due to misconfigured firmware image, timing 
        /// problems stemming from board hardware failures, or because the flash has exceeded its limitations for writes. If this error is detected by the host, it may switch 
        /// to attempt to read the devices OEM configuration area and save it on the host in order to preserve device specific settings.
        /// </summary>
        IO_OEMCFG_FLUSH = 0x040B,

        /// <summary>
        /// This is set by the OEM Configuration module when it fails to detect the correct low level file system headers for the OEM configuration area. This means that the 
        /// OEM configuration area has not been formatted - due to a misconfigured board or that the OEM Configuration area has become corrupt and should not be trusted 
        /// without attempting recovery or reconfiguration.
        /// </summary>
        IO_OEMCFG_FORMAT = 0x040C,

        /// <summary>
        /// This is set by the CPU module when an attempt is made to configure reserved IO pins. This is likely due to a misconfigured firmware build or errant Indy Firmware code.
        /// </summary>
        IO_INVAL_IORSVD = 0x040D,

        /// <summary>
        /// This is set by the OEM Configuration module when an invalid string type is selected.
        /// </summary>
        IO_OEMCFG_STRING_TYPE = 0x040E,

        /// <summary>
        /// This is set by the OEM Configuration module when an invalid string length is entered
        /// </summary>
        IO_OEMCFG_STRING_LENGTH = 0x040F,

        /// <summary>
        /// This is set by the OEM Configuration module when an invalid character is entered.
        /// </summary>
        IO_OEMCFG_STRING_CHARACTER = 0x0410,

        /// <summary>
        /// This is set by the OEM Configuration module when an string read cannot be read correctly since the current string has an invalid header.
        /// </summary>
        IO_OEMCFG_STRING_CURRENT_INVALID = 0x0411,

        /// <summary>
        /// This is set by the OEM Configuration module when the generated key does not match the check key when attempting to format the OEM Configuration space.
        /// </summary>
        IO_OEMCFG_FORMAT_KEY_INVALID = 0x0412,

        /// <summary>
        /// This is set by the OEM Configuration module when an invalid format configuration is specified.
        /// </summary>
        IO_OEMCFG_FORMAT_CONFIGURATION_INVALID = 0x0413,

        /// <summary>
        /// This is set by the CPU module while attempting to lock or unlock a flash sector and the specified sector is invalid.
        /// </summary>
        IO_INVAL_NV_SECTOR = 0x0414,

        /// <summary>
        /// This is set by the UHF RFID Transceiver interface module when an UHF RFID Transceiver register read, when configured for Serial port mode, returns the incorrect register 
        /// address in the serial response frame. This could be due to board or UHF RFID Transceiver hardware problems or errant Indy Firmware code.
        /// </summary>
        TILDENIF_ERR_ADDRMISMAT = 0x0601,

        /// <summary>
        /// This is set by the UHF RFID Transceiver interface module when failsafe logic is activated due to no response from UHF RFID Transceiver. This happens on UHF RFID 
        /// Transceiver register reads. This could be due to board or UHF RFID Transceiver hardware problems.
        /// </summary>
        TILDENIF_ERR_RDFAILSAFE = 0x0602,

        /// <summary>
        /// Set by the low level interface logic if, during power management, an invalid power state is requested. This will likely only occur if the system is corrupt
        /// </summary>
        TILDENIF_ERR_INVALPWRST = 0x0603,

        /// <summary>
        /// Set by the low level interface logic if, during a write, an invalid setting is selected.  Diagnostics: Address, Data, Id
        /// </summary>
        TILDENIF_ERR_INVALID_SETTING_R500 = 0x0604,

        /// <summary>
        /// This error code is set during firmware boot when the Built In Self Test code is executed. This error indicates that certain register power up defaults on UHF RFID 
        /// Transceiver were not detected - possibly indicating a hardware problem.
        /// </summary>
        BIST_ERR_RF_IO_REG_CHK = 0x0701,

        /// <summary>
        /// This error code is set during firmware boot when the Built In Self Test code is executed. This error indicates that a walking 1's or walking 0's bus test failed - 
        /// possibly indicating a hardware problem
        /// </summary>
        BIST_ERR_RF_REG_BITS = 0x0702
    }
}
