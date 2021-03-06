﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using rfid;
using rfid.Constants;
using rfid.Structures;
using Serilog;

namespace RfidConsole
{
    /// <summary>
    /// Exercices the Linkage library.
    /// </summary>
    /// <remarks>
    /// Essentially this code is a more structured version of the example code in the SDK at D:\Code\indy_sdk\host\rfid_library\samples\csharp\Example\Example.cs
    /// The Linkage library source code is in the SDK at ...\indy_sdk\host\rfid_library\samples\csharp\Linkage\Common\Linkage.cs
    /// For documentation of the underlying methods see IN_DG_IPJ_Indy_Host_Library_API_Reference_Manual_v2_6_0_20120504_R1.pdf
    /// </remarks>
    public class Rfiddler: IDisposable
    {
        private readonly Options options;
        private readonly ILogger logger = Log.Logger.ForContext<Rfiddler>();
        private readonly Linkage link = new Linkage();

        static Int32 _callbackCount;
        static uint enableSelectCriteria = 0x00; // set to 0x01 to enable
        static uint enablePostSingulationMatch = 0x00; // set to 0x02 to enable
        private bool enableTagLock = false;
        private bool enableTagKill = false;

        private bool disposed;

        public Rfiddler(Options options)
        {
            this.options = options;
        }

        public void Start()
        {
            Startup(LibraryMode.DEFAULT);

            RadioEnumeration radios = RetrieveAttachedRadiosList();

            int radioHandle = RadioOpen(radios);

            RadioClose(radioHandle);

            radioHandle = RadioOpen(radios);

            RadioGetOperationMode(radioHandle);

            GetMacVersion(radioHandle);

            MacGetBootLoaderVersion(radioHandle);

            RadioSetOperationMode(radioHandle, RadioOperationMode.NONCONTINUOUS);

            RadioGetOperationMode(radioHandle);

            RadioGetPowerState(radioHandle);

            RadioSetPowerState(radioHandle, RadioPowerState.FULL);

            RadioGetPowerState(radioHandle);

            RadioSetPowerState(radioHandle, RadioPowerState.STANDBY);

            RadioGetPowerState(radioHandle);

            EnumerateLinkProfiles(radioHandle);

            GetAntennaPortStatus(radioHandle);

            SetAntennaPortStatus(radioHandle, AntennaPortState.DISABLED);

            GetAntennaPortStatus(radioHandle);

            SetAntennaPortStatus(radioHandle, AntennaPortState.ENABLED);

            GetAntennaPortStatus(radioHandle);

            AntennaPortGetConfiguration(radioHandle);

            AntennaPortSetConfiguration(radioHandle, (uint)options.DwellTime, (uint)options.PowerLevel);

            var selectCriteria = Set18K6CSelectCriteria(radioHandle);

            Get18K6CSelectCriteria(radioHandle, selectCriteria);

            Set18K6CQueryTagGroup(radioHandle);

            ConfigureImpinjExtensions(radioHandle);

            Set18K6CPostMatchCriteria(radioHandle);

            Set18K6CSingulationAlgorithmParameters(radioHandle);

            //result = link.RadioSetResponseDataMode( radioHandle, ResponseType.DATA, ResponseMode.EXTENDED );
            //logger.Information( "link.RadioSetResponseDataMode => " + result );

            Tag18K6CInventory(radioHandle);

            Tag18K6CRead(radioHandle);

            Tag18K6CQT(radioHandle);

            Tag18K6CBlockErase(radioHandle);

            Tag18K6CKill(radioHandle);

            Tag18K6CLock(radioHandle);

            RadioTurnCarrierWaveOnRandom(radioHandle);

            RegisterAccess(radioHandle);

            RadioClose(radioHandle);

            MacReset();
        }

        private void Startup(LibraryMode mode)
        {
            var version = new LibraryVersion();

            var result = link.Startup(version, mode);

            logger.Information("link.Startup({Mode}) => {Result}", mode, result);
            logger.Information("LibraryVersion {@Version}", new { version.major, version.minor, version.maintenance, version.release });
        }

        private RadioEnumeration RetrieveAttachedRadiosList()
        {
            RadioEnumeration radios = new RadioEnumeration();

            Result result = link.RetrieveAttachedRadiosList(radios, 0);

            logger.Information("link.RetrieveAttachedRadiosList => {Result}", result);
            logger.Information("RadioEnum.length: {Length}, RadioEnum.totalLength: {TotalLength}, RadioEnum.countRadios: {CountRadios}", radios.length, radios.totalLength, radios.countRadios);

            for (int index = 0; index < radios.radioInfo.Length; ++index)
            {
                var version = radios.radioInfo[index].driverVersion;
                var uniqueId = radios.radioInfo[index].uniqueId;

                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.length : " + radios.radioInfo[index].length);
                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.driverVersion : {@Version}", new { version.major, version.minor, version.maintenance, version.release });
                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.cookie   : " + radios.radioInfo[index].cookie);
                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.idLength : " + radios.radioInfo[index].idLength);
                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.uniqueId : {UniqueId}", Encoding.ASCII.GetString(uniqueId, 0, uniqueId.Length));
            }

            return radios;
        }

        private int RadioOpen(RadioEnumeration radios)
        {
            var radioHandle = 0;

            Result result = link.RadioOpen(radios.radioInfo[0].cookie, ref radioHandle, MacMode.DEFAULT);

            logger.Information("link.RadioOpen => {Result}", result);
            logger.Information("Cookie: {Cookie}, radio handle: {Handle}", radios.radioInfo[0].cookie, radioHandle);

            return radioHandle;
        }

        private void RadioClose(int radioHandle)
        {
            Result result = link.RadioClose(radioHandle);

            logger.Information("link.RadioClose => {Result}", result);
        }

        private void RegisterAccess(int radioHandle)
        {
            // test a banked register write            
            var result = link.MacWriteBankedRegister(radioHandle, 0x704, 2, 0xA5A5);
            logger.Information("link.MacWriteBankedRegister => {Result}", result);

            // should give an error, due to invalid selector
            result = link.MacWriteBankedRegister(radioHandle, 0x704, 16, 0xA5A5);
            logger.Information("link.MacWriteBankedRegister result (Expect Invalid Parameter) => {Result}", result);


            // test a banked register read, last iteration i=16 should give an error
            UInt32 value = 0;
            for (UInt16 i = 0; i < 17; i++)
            {
                result = link.MacReadBankedRegister(radioHandle, 0x704, i, ref value);
                if (i < 16)
                {
                    logger.Information("link.MacReadBankedRegister => {Result}", result);
                }
                else
                {
                    logger.Information("link.MacReadBankedRegister result (Expect Invalid Parameter) => {Result}", result);
                }
            }

            // Test OEMConfig Read/Write            
            UInt32[] readData = new UInt32[20];
            UInt32 numToRead = 16;
            result = link.MacReadOemData(radioHandle, 0x1f70, ref numToRead, readData);
            logger.Information("link.MacReadOemData => " + result + ", numRead=" + numToRead);

            numToRead++;
            result = link.MacReadOemData(radioHandle, 0x1f70, ref numToRead, readData);
            logger.Information("link.MacReadOemData result (Expect Invalid Parameter) : " + result + ", numRead=" + numToRead);

            UInt32 numToWrite = 16;
            result = link.MacWriteOemData(radioHandle, 0x1f70, ref numToWrite, readData);
            logger.Information("link.MacWriteOemData => " + result + ", numWritten=" + numToWrite);

            numToWrite++;
            result = link.MacWriteOemData(radioHandle, 0x1f70, ref numToWrite, readData);
            logger.Information("link.MacWriteOemData result (Expect Invalid Parameter) : " + result + ", numWritten=" + numToWrite);


            // force a mac error writing an invalid address and confirm mac error and macLastError
            // can be retrieved and reflect the same value
            result = link.MacWriteRegister(radioHandle, 0xFFFF, 0);
            logger.Information("link.MacWriteRegister result (Expect Invalid Parameter) : {Result}", result);
            UInt32 macError = 0;
            UInt32 macLastError = 0;
            result = link.MacGetError(radioHandle, ref macError, ref macLastError);
            logger.Information("link.MacGetError (" + macError + ", " + macLastError + ") => {Result}", result);

            // confirm a successful register access clears the mac error but not the lastMacError
            UInt32 macVer = 0;
            result = link.MacReadRegister(radioHandle, 0x0000, ref macVer);
            logger.Information("link.MacReadRegister => {Result}", result);
            result = link.MacGetError(radioHandle, ref macError, ref macLastError);
            logger.Information("link.MacGetError (" + macError + ", " + macLastError + ") => {Result}", result);
        }

        private void RadioTurnCarrierWaveOnRandom(int radioHandle)
        {
            var program = new Rfiddler(options);

            RandomCwParms randomCwParms = new RandomCwParms();

            // Enable Tx Random Data status packet by setting bits 7 and 0 in HST_CMNDIAGS (0x201)           
            UInt32 hstCmnDiags = 0;
            var result = link.MacReadRegister(radioHandle, 0x0201, ref hstCmnDiags);
            logger.Information("link.MacReadRegister => {Result}", result);
            hstCmnDiags |= 0x81;
            result = link.MacWriteRegister(radioHandle, 0x0201, hstCmnDiags);
            logger.Information("link.MacWriteRegister => {Result}", result);

            logger.Information("Starting random cw test:");
            logger.Information($"Limiting to {options.PacketCount} MAC packets");

#if TEST_NULL_RANDOM_CW_CALLBACK
            randomCwParms.callback = null;
            randomCwParms.context = (IntPtr) null;
            randomCwParms.callbackCode = (IntPtr) null;
#else
            randomCwParms.callback = program.PacketCallback;
            randomCwParms.context = IntPtr.Zero;
            randomCwParms.callbackCode = IntPtr.Zero;
#endif
            randomCwParms.duration = 10000;

            result = link.RadioTurnCarrierWaveOnRandom(radioHandle, randomCwParms);

            logger.Information("link.RadioTurnCarrierWaveOnRandom => {Result}", result);
        }

        private void Tag18K6CLock(int radioHandle)
        {
            var program = new Rfiddler(options);

            // LOCK check - do not want to accidentally have tags locked unexpectedly...
            if (!program.enableTagLock)
            {
                return;
            }

            LockParms lockParms = new LockParms();

            lockParms.common.tagStopCount = 0;
            lockParms.common.callback = program.PacketCallback;
            lockParms.common.context = IntPtr.Zero;
            lockParms.common.callbackCode = IntPtr.Zero;

            lockParms.lockCmdParms.permissions.killPasswordPermissions = PasswordPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.accessPasswordPermissions = PasswordPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.epcMemoryBankPermissions = MemoryPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.tidMemoryBankPermissions = MemoryPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.userMemoryBankPermissions = MemoryPermission.NO_CHANGE;

            lockParms.accessPassword = 0x0;

            var result = link.Tag18K6CLock(radioHandle, lockParms, 0);

            logger.Information("link.Tag18K6CLock => {Result}", result);
        }

        private void Tag18K6CKill(int radioHandle)
        {
            var program = new Rfiddler(options);

            // KILL check - do not want to accidentally have tags killed unexpectedly...
            if (!program.enableTagKill)
            {
                return;
            }

            KillParms killParms = new KillParms();

            killParms.common.tagStopCount = 0;
            killParms.common.callback = program.PacketCallback;
            killParms.common.context = IntPtr.Zero;
            killParms.common.callbackCode = IntPtr.Zero;

            killParms.accessPassword = 0x0;
            killParms.killCmdParms.killPassword = 0x0;

            var result = link.Tag18K6CKill(radioHandle, killParms, 0);

            logger.Information("link.Tag18K6CKill => {Result}", result);
        }

        private void Tag18K6CBlockErase(int radioHandle)
        {
            var program = new Rfiddler(options);

            BlockEraseParms blockEraseParms = new BlockEraseParms();

            logger.Information($"Starting basic block erase test, limiting of {options.PacketCount} MAC packets");

            blockEraseParms.common.tagStopCount = 0;
            blockEraseParms.common.callback = program.PacketCallback;
            blockEraseParms.common.context = IntPtr.Zero;
            blockEraseParms.common.callbackCode = IntPtr.Zero;

            blockEraseParms.blockEraseCmdParms.bank = MemoryBank.RESERVED;
            blockEraseParms.blockEraseCmdParms.offset = 0;
            blockEraseParms.blockEraseCmdParms.count = 4;

            blockEraseParms.accessPassword = 0x0;

            var result = link.Tag18K6CBlockErase(radioHandle, blockEraseParms, 0);

            logger.Information("link.Tag18K6CBlockErase => {Result}", result);
        }

        // ReSharper disable once InconsistentNaming
        private void Tag18K6CQT(int radioHandle)
        {
            var program = new Rfiddler(options);

            QTParms qtParms = new QTParms();


            logger.Information($"Starting basic QT test, limiting of {options.PacketCount} MAC packets");

            qtParms.common.tagStopCount = 0;
            qtParms.common.callback = program.PacketCallback;
            qtParms.common.context = IntPtr.Zero;
            qtParms.common.callbackCode = IntPtr.Zero;

            qtParms.optCmdType = OptType.OPT_READ;

            ReadCmdParms readCmdParms = new ReadCmdParms();

            readCmdParms.bank = MemoryBank.EPC;
            readCmdParms.offset = 2;
            readCmdParms.count = 6;

            qtParms.accessParms = readCmdParms;

            qtParms.qtCmdParms.qtMemoryMap = QTMemMapType.PRIVATE;
            qtParms.qtCmdParms.qtPersistence = QTPersistenceType.PERMANENT;
            qtParms.qtCmdParms.qtReadWrite = QTCtrlType.READ;
            qtParms.qtCmdParms.qtShortRange = QTShortRangeType.DISABLE;

            qtParms.accessPassword = 0x0;

            var result = link.Tag18K6CQT(radioHandle, qtParms, 0);

            logger.Information("link.Tag18K6CQT => {Result}", result);
        }

        private void Tag18K6CRead(int radioHandle)
        {
            Rfiddler program = new Rfiddler(options);

            ReadParms readParms = new ReadParms();

            logger.Information($"Starting basic read test, limiting of {options.PacketCount} MAC packets");

            readParms.common.tagStopCount = 0;
            readParms.common.callback = program.PacketCallback;
            readParms.common.context = IntPtr.Zero;
            readParms.common.callbackCode = IntPtr.Zero;

            readParms.readCmdParms.bank = MemoryBank.EPC;
            readParms.readCmdParms.offset = 2;
            readParms.readCmdParms.count = 6;

            readParms.accessPassword = 0x0;

            var result = link.Tag18K6CRead(radioHandle, readParms, 0);

            logger.Information("link.Tag18K6CRead => {Result}", result);
        }

        private void Tag18K6CInventory(int radioHandle)
        {
            logger.Information($"Starting basic inventory test, limiting of {options.PacketCount} MAC packets");

            Rfiddler program = new Rfiddler(options);

            InventoryParms inventoryParms = new InventoryParms();

            inventoryParms.common = new CommonParms();

            inventoryParms.common.tagStopCount = 0;
            inventoryParms.common.callback = program.PacketCallback;
            inventoryParms.common.context = IntPtr.Zero;
            inventoryParms.common.callbackCode = IntPtr.Zero;

            var result = link.Tag18K6CInventory(radioHandle, inventoryParms, enableSelectCriteria | enablePostSingulationMatch);

            logger.Information("link.Tag18K6CInventory => {Result}", result);
        }

        private void Set18K6CSingulationAlgorithmParameters(int radioHandle)
        {
            // Fixed.

            // See RFID_18K6C_SINGULATION_FIXEDQ_PARMS
            FixedQParms fqp = new FixedQParms
            {
                qValue = 1,
                retryCount = 1,
                repeatUntilNoTags = 1,
                toggleTarget = options.IsTagFocusEnabled ? 0U : 1
            };

            var result = link.Set18K6CSingulationAlgorithmParameters(radioHandle, SingulationAlgorithm.FIXEDQ, fqp);

            logger.Information("link.Set18K6CSingulationAlgorithmParameters Fixed Q => {Result}", result);

            // Dynamic.

            DynamicQParms dqp = new DynamicQParms
            {
                startQValue = 3,
                minQValue = 0,
                maxQValue = 7,
                retryCount = 1,
                thresholdMultiplier = 4,
                toggleTarget = options.IsTagFocusEnabled ? 0U : 1
            };

            result = link.Set18K6CSingulationAlgorithmParameters(radioHandle, SingulationAlgorithm.DYNAMICQ, dqp);

            logger.Information("link.Set18K6CSingulationAlgorithmParameters Dynamic Q => {Result}", result);
        }

        private void Set18K6CPostMatchCriteria(int radioHandle)
        {
            SingulationCriteria singulationCriteria = new SingulationCriteria();

            singulationCriteria.countCriteria = 1;

            singulationCriteria.pCriteria = new SingulationCriterion[singulationCriteria.countCriteria];

            singulationCriteria.pCriteria[0] = new SingulationCriterion();

            singulationCriteria.pCriteria[0].match = 1; // EPC data must match the mask to be returned
            // set to 0 if you EPC's that don't match

            // change the mask attributes as appropriate for the specific tag
            singulationCriteria.pCriteria[0].mask.offset = 80; // bit offset in the EPC of the 6th word
            singulationCriteria.pCriteria[0].mask.count = 16; // match 16 bits starting at offset
            singulationCriteria.pCriteria[0].mask.mask[0] = 0x00; // first byte of 6th word is 0
            singulationCriteria.pCriteria[0].mask.mask[1] = 0x24; // second byte of 6th word is 0x24

            var result = link.Set18K6CPostMatchCriteria(radioHandle, singulationCriteria, 0);

            logger.Information("link.Set18K6CPostMatchCriteria => {Result}", result);
        }

        private void ConfigureImpinjExtensions(int radioHandle)
        {
            var extensions = new ImpinjExtensions();

            var result = link.RadioGetImpinjExtensions(radioHandle, extensions);
            logger.Information("link.RadioGetImpinjExtensions => {Result}", result);
            logger.Information("ImpinjExtensions: {@Extensions}", new { extensions.fastId, extensions.tagFocus, extensions.blockWriteMode });

            // When performing Inventory operations, if a tag is singulated, this extension is enabled, and the additional pre-requisite inventory 
            // controls have been configured, the tag will be suppressed in the tag population for the duration of the inventory operation.
            extensions.tagFocus = options.IsTagFocusEnabled ? TagFocus.FOCUS_ENABLED : TagFocus.FOCUS_DISABLED;

            // When performing Inventory operations, if a tag is singulated and this extension is enabled, then the Tag TID memory is returned along with the Tag EPC data.
            extensions.fastId = options.IsFastIdEnabled ? FastId.FAST_ID_ENABLED : FastId.FAST_ID_DISABLED;

            extensions.blockWriteMode = BlockWriteMode.AUTO;

            result = link.RadioSetImpinjExtensions(radioHandle, extensions);
            logger.Information("link.RadioSetImpinjExtensions => {Result}", result);

            result = link.RadioGetImpinjExtensions(radioHandle, extensions);
            logger.Information("link.RadioGetImpinjExtensions => {Result}", result);
            logger.Information("ImpinjExtensions: {@Extensions}", new { extensions.fastId, extensions.tagFocus, extensions.blockWriteMode });
        }

        private void Set18K6CQueryTagGroup(int radioHandle)
        {
            // Configure the query tag group to only get tags with the SL asserted
            var group = new TagGroup
            {
                // Specifies the state of the selected (SL) flag for tags that will have the operation applied to them.
                selected = 0 == enableSelectCriteria ? Selected.SELECT_ALL : Selected.SELECT_ASSERTED,

                // Specifies which inventory session flag (S0, S1, S2, or S3) will be matched against the inventory state specified by target.
                session = options.IsTagFocusEnabled ? Session.S1 : Session.S2,

                // Specifies the state of the inventory session flag (A or B), specified by session, for tags that will have the operation applied to them.
                target = SessionTarget.A
            };

            var result = link.Set18K6CQueryTagGroup(radioHandle, group);

            logger.Information("link.Set18K6CQueryTagGroup => {Result}", result);
        }

        private void Get18K6CSelectCriteria(int radioHandle, SelectCriteria selectCriteria)
        {
            Result result = link.Get18K6CSelectCriteria(radioHandle, selectCriteria);

            logger.Information("link.Get18K6CSelectCriteria => {Result}", result);
        }

        private SelectCriteria Set18K6CSelectCriteria(int radioHandle)
        {
            SelectCriteria selectCriteria = new SelectCriteria();

            selectCriteria.countCriteria = 1;

            selectCriteria.pCriteria = new SelectCriterion[selectCriteria.countCriteria];

            selectCriteria.pCriteria[0] = new SelectCriterion();

            // change the mask attributes as appropriate for the specific tag
            selectCriteria.pCriteria[0].mask.bank = MemoryBank.EPC;
            selectCriteria.pCriteria[0].mask.offset = 112; // bit offset in the EPC memory bank, for the 6th word of the EPC
            selectCriteria.pCriteria[0].mask.count = 16; // 16 bits to match starting at offset
            selectCriteria.pCriteria[0].mask.mask[0] = 0x00; // first byte of 6th word is 0
            selectCriteria.pCriteria[0].mask.mask[1] = 0x24; // 2nd byte of 6th word is 24

            selectCriteria.pCriteria[0].action.target = Target.SELECTED;
            selectCriteria.pCriteria[0].action.action = rfid.Constants.Action.ASLINVA_DSLINVB;
            selectCriteria.pCriteria[0].action.enableTruncate = 0;


            var result = link.Set18K6CSelectCriteria(radioHandle, selectCriteria, 0);

            logger.Information("link.Set18K6CSelectCriteria => {Result}", result);

            return selectCriteria;
        }

        private void GetAntennaPortStatus(int radioHandle)
        {
            AntennaPortStatus antennaPortStatus = new AntennaPortStatus();

            var result = link.AntennaPortGetStatus(radioHandle, 0, antennaPortStatus);

            logger.Information("link.AntennaPortGetStatus => {Result}", result);
            logger.Information("\tRadioHandle used       : " + radioHandle);
            logger.Information("\tLength found           : " + antennaPortStatus.length);
            logger.Information("\tState found            : " + antennaPortStatus.state);
            logger.Information("\tSense Value found      : " + antennaPortStatus.antennaSenseValue);
        }

        private void SetAntennaPortStatus(int radioHandle, AntennaPortState portState)
        {
            Result result = link.AntennaPortSetState(radioHandle, 0, portState);

            logger.Information("link.AntennaPortSetStatus ( {PortState} ) => {Result}", portState, result);
        }

        private void AntennaPortGetConfiguration(int radioHandle)
        {
            AntennaPortConfig antennaPortConfig = new AntennaPortConfig();

            var result = link.AntennaPortGetConfiguration(radioHandle, 0, antennaPortConfig);

            logger.Information("link.AntennaPortGetConfig => {Result}", result);
            logger.Information("\tRadioHandle used              : " + radioHandle);
            logger.Information("\tLength found                  : " + antennaPortConfig.length);
            logger.Information("\tPowerLevel found              : " + antennaPortConfig.powerLevel);
            logger.Information("\tDwellTime found               : " + antennaPortConfig.dwellTime);
            logger.Information("\tNumberInventoryCycles found   : " + antennaPortConfig.numberInventoryCycles);
            logger.Information("\tphysicalRxPort found          : " + antennaPortConfig.physicalRxPort);
            logger.Information("\tPhysicalTxPort found          : " + antennaPortConfig.physicalTxPort);
            logger.Information("\tSense Threshold( glob ) found : " + antennaPortConfig.antennaSenseThreshold);
        }

        private void AntennaPortSetConfiguration(int radioHandle, uint dwellTime, uint powerLevel)
        {
            if (powerLevel > 330)
            {
                throw new ArgumentOutOfRangeException(nameof(powerLevel), "Maximum allowed is 330");
            }

            AntennaPortConfig antennaPortConfig = new AntennaPortConfig();

            link.AntennaPortGetConfiguration(radioHandle, 0, antennaPortConfig);

            antennaPortConfig.dwellTime = dwellTime;

            // The power level for the logical antenna port’s physical transmit antenna, specified in 1/10th dBm
            // Default is 300 (30 dBm), absolute maximum is 330 (33 dBm)
            antennaPortConfig.powerLevel = powerLevel;

            Result result = link.AntennaPortSetConfiguration(radioHandle, 0, antennaPortConfig);

            logger.Information("link.AntennaPortSetConfiguration => {Result}", result);
        }

        private void EnumerateLinkProfiles(int radioHandle)
        {
            UInt32 currentLinkProfile = 0;

            link.RadioGetCurrentLinkProfile(radioHandle, ref currentLinkProfile);
            var savedLinkProfile = currentLinkProfile;

            Result result;

            for (currentLinkProfile = 0; currentLinkProfile < 4; ++currentLinkProfile)
            {
                result = link.RadioSetCurrentLinkProfile(radioHandle, currentLinkProfile);

                logger.Information("link.RadioSetCurrentLinkProfile ( " + currentLinkProfile + " ) => {Result}", result);
                logger.Information("\tRadioHandle used           : " + radioHandle);


                result = link.RadioGetCurrentLinkProfile(radioHandle, ref currentLinkProfile);

                logger.Information("link.RadioGetCurrentLinkProfile => {Result}", result);
                logger.Information("\tRadioHandle used           : " + radioHandle);
                logger.Information("\tCurrent Link Profile found : " + currentLinkProfile);
            }


            RadioLinkProfile linkProfile = new RadioLinkProfile();

            result = link.RadioGetLinkProfile(radioHandle, currentLinkProfile, linkProfile);

            logger.Information("link.RadioGetLinkProfile => {Result}", result);
            logger.Information("\tFound length           : " + linkProfile.length);
            logger.Information("\tFound profileId        : " + linkProfile.profileId);
            logger.Information("\tFound profileVersion   : " + linkProfile.profileVersion);
            logger.Information("\tFound profileProtocol  : " + linkProfile.profileProtocol);

            logger.Information("\tFound iso length              : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).length);
            logger.Information("\tFound iso modulationType      : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).modulationType);
            logger.Information("\tFound iso tari                : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).tari);
            logger.Information("\tFound iso data01Difference    : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).data01Difference);
            logger.Information("\tFound iso pulseWidth          : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).pulseWidth);
            logger.Information("\tFound iso rtCalibration       : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).rtCalibration);
            logger.Information("\tFound iso trCalibration       : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).trCalibration);
            logger.Information("\tFound iso divideRatio         : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).divideRatio);
            logger.Information("\tFound iso millerNumber        : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).millerNumber);
            logger.Information("\tFound iso trLinkFrequency     : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).trLinkFrequency);
            logger.Information("\tFound iso varT2Delay          : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).varT2Delay);
            logger.Information("\tFound iso rxDelay             : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).rxDelay);
            logger.Information("\tFound iso minT2Delay          : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).minT2Delay);
            logger.Information("\tFound iso txPropogationDelay  : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).length);

            // Restore link profile
            result = link.RadioSetCurrentLinkProfile(radioHandle, savedLinkProfile);

            logger.Information("link.RadioSetCurrentLinkProfile ( " + savedLinkProfile + " ) => {Result}", result);
            logger.Information("\tRadioHandle used           : " + radioHandle);
        }

        private void RadioGetPowerState(int radioHandle)
        {
            var powerState = RadioPowerState.UNKNOWN;
            var result = link.RadioGetPowerState(radioHandle, ref powerState);

            logger.Information("link.RadioGetPowerState => {Result}", result);
            logger.Information("\tRadioHandle used    : " + radioHandle);
            logger.Information("\tPowerState found    : {PowerState}", powerState);
        }

        private void RadioSetPowerState(int radioHandle, RadioPowerState powerState)
        {
            Result result = link.RadioSetPowerState(radioHandle, powerState);

            logger.Information("link.RadioSetPowerState ( {PowerState} ) => {Result}", powerState, result);
        }

        private void RadioGetOperationMode(int radioHandle)
        {
            var operationMode = new RadioOperationMode();

            var result = link.RadioGetOperationMode(radioHandle, ref operationMode);

            logger.Information("link.RadioGetOperationMode => {Result}", result);
            logger.Information("\tRadioHandle used    : " + radioHandle);
            logger.Information("\tOperationMode found : " + operationMode);
        }

        private void RadioSetOperationMode(int radioHandle, RadioOperationMode operationMode)
        {
            Result result = link.RadioSetOperationMode(radioHandle, operationMode);

            logger.Information("link.RadioSetOperationMode ( {OperationMode} ) => {Result}", operationMode, result);
        }

        private void MacGetBootLoaderVersion(int radioHandle)
        {
            MacBootLoaderVersion blVersion = new MacBootLoaderVersion();
            var result = link.MacGetBootLoaderVersion(radioHandle, blVersion);

            logger.Information("link.MacGetBootLoaderVersion => {Result}", result);
            logger.Information("MacVersion {@Version}", new { blVersion.major, blVersion.minor, blVersion.maintenance, blVersion.release });
        }

        private void GetMacVersion(int radioHandle)
        {
            MacVersion macVersion = new MacVersion();
            var result = link.MacGetVersion(radioHandle, macVersion);

            logger.Information("link.MacGetVersion => {Result}", result);
            logger.Information("MacVersion {@Version}", new { macVersion.major, macVersion.minor, macVersion.maintenance, macVersion.release });
        }

        /// <summary>
        /// Reset the RFID Radio Module to restore the default configuration.
        /// </summary>
        /// <remarks>
        /// Rfiddler may not restore all configuration changes, therefore it's best to reset the device to the default state.
        /// </remarks>
        private void MacReset()
        {
            RadioEnumeration radios = RetrieveAttachedRadiosList();

            int radioHandle = RadioOpen(radios);

            Result result = link.MacReset(radioHandle, MacResetType.SOFT);

            logger.Information("link.MacReset => {Result}", result);
        }

        private int PacketCallback([In] int handle, [In] uint bufferLength, [In] IntPtr pBuffer, [In, Out] IntPtr context)
        {
            // Get common packet values.
            // From the Impinj documentation:
            //----------------------------------------------------------------------------------------------------------------------------
            // Bytes    Value   Name            Description
            //----------------------------------------------------------------------------------------------------------------------------
            // 0:0      1       Pkt_Version     Packet version
            // 1:1      -       Pkt_Flags       Packet flags (0 = Operation NOT continuous mode, 1 = Operation exected in continuous mode)
            // 3:2      0x0000  Pkt_Type        Packet Type Value
            // 5:4      2       Pkt_Length      Packet Length Value
            // 7:6      0       Pkt_Reserved    Packet Reserved
            // 11:8     -       Command         MAC Command that initiated the packet sequence. See Mac Commands for details.
            // 15:12    -       MS_Ctr          Firmware millisecond counter when operation started

            var packetBuffer = new byte[bufferLength];

            Marshal.Copy(pBuffer, packetBuffer, 0, (int)bufferLength);

            var packetFlags = packetBuffer[1];
            
            var packetType = (PacketType)(short)((packetBuffer[3] << 8) | packetBuffer[2]);

            var packetLength = (short)((packetBuffer[5] << 8) | packetBuffer[4]);
            var packetTypeString = $"Mac Packet received, PacketType = 0x{(int)packetType:X4} ({packetType})";

            logger.Information(packetTypeString);

            if (packetType == PacketType.Inventory)
            {
                var length = (short)(((packetLength - 3) * 4) - (packetFlags >> 6));
                string packetEpcString = "EPC = ";
                for (int index = 0; index < length; ++index)
                {
                    packetEpcString += $"{packetBuffer[20 + index]:X2}";
                }

                logger.Verbose(packetEpcString);
            }
            else if (packetType == PacketType.TagAccess)
            {
                // access packet, print the flag word if non-zero, along with error indicators

                var flagWord = packetBuffer[1];
                var packetFlagString = $"AccessPacket Flag = 0x{flagWord:X2}";

                if (0 != (flagWord & 0x03)) // some error occurred
                {
                    if (0 != (flagWord & 0x0E)) // backscatter error, reported in the tag_error_code
                    {
                        packetFlagString += $", Error = 0x{packetBuffer[13]:X2}";
                    }
                    else // MAC protocol access error, reported prot_error_code field
                    {
                        packetFlagString += $", Error = 0x{packetBuffer[15]:X2}{packetBuffer[14]:X2}";
                    }

                    logger.Warning(packetFlagString);
                }
            }
            else if (packetType == PacketType.CommandEnd)
            {
                // if its an end packet, print the status string too.
                var macErrorCode = (MacErrorCode)GetField(packetBuffer, 3, 12);
                if (macErrorCode != MacErrorCode.MACERR_SUCCESS)
                {
                    logger.Warning("MAC error code {MacErrorCode} returned by reader", macErrorCode);
                }

                logger.Information($"EndPacket Status = 0x{packetBuffer[15]:X2}{packetBuffer[14]:X2}{packetBuffer[13]:X2}{packetBuffer[12]:X2}");
            }

            ++_callbackCount;

            return options.PacketCount == _callbackCount ? 1 : 0;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            var result = link.Shutdown();

            logger.Information("link.Shutdown => {Result}", result);

            disposed = true;
        }

        /// <summary>
        /// Gets data in a field of an Indy Host Library return packet.
        /// </summary>
        private static uint GetField(byte[] packetBuffer, short bytesToRead, uint offsetPosition)
        {
            const int numberOfBitsInByte = 8;

            if (offsetPosition + bytesToRead > packetBuffer.Length)
            {
                throw new ArgumentException("GetDataFromBuffer - offsetPosition + bytesToRead will exceed packetBuffer length");
            }

            uint value = 0;

            for (int i = 0; i < bytesToRead; i++)
            {
                // Get the byte at the offset position and bit shift.
                value += (uint)(packetBuffer[offsetPosition + i] << (i * numberOfBitsInByte));
            }

            return value;
        }
   }
}