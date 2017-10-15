using System;
using System.Runtime.InteropServices;
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
    /// </remarks>
    public class Rfiddler: IDisposable
    {
        private readonly ILogger logger = Log.Logger.ForContext<Rfiddler>();
        private readonly Linkage link = new Linkage();

        static Int32 _callbackCount;
        static uint enableSelectCriteria = 0x00; // set to 0x01 to enable
        static uint enablePostSingulationMatch = 0x00; // set to 0x02 to enable
        static uint enableTagFocus = 0x00; // set to 0x01 to enable
        static uint enableTagSuppression = 0x00; // set to 0x01 to enable
        static uint enableFastId = 0x00; // set to 0x01 to enable
        private bool enableTagLock = false;
        private bool enableTagKill = false;
        private bool disposed;

        public void Start(string[] args)
        {
            Startup();

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

            SetRadioPowerState(radioHandle, RadioPowerState.FULL);

            RadioGetPowerState(radioHandle);

            SetRadioPowerState(radioHandle, RadioPowerState.STANDBY);

            RadioGetPowerState(radioHandle);

            EnumerateLinkProfiles(radioHandle);

            GetAntennaPortStatus(radioHandle);

            SetAntennaPortStatus(radioHandle, AntennaPortState.DISABLED);

            GetAntennaPortStatus(radioHandle);

            SetAntennaPortStatus(radioHandle, AntennaPortState.ENABLED);

            GetAntennaPortStatus(radioHandle);

            AntennaPortGetConfiguration(radioHandle);

            AntennaPortSetConfiguration();

            var selectCriteria = Set18K6CSelectCriteria(radioHandle);

            Get18K6CSelectCriteria(radioHandle, selectCriteria);

            Set18K6CQueryTagGroup(radioHandle);

            UseImpinjExtensions(radioHandle);

            Set18K6CPostMatchCriteria(radioHandle);

            Set18K6CSingulationAlgorithmParameters(radioHandle);

            //result = link.RadioSetResponseDataMode( radioHandle, ResponseType.DATA, ResponseMode.EXTENDED );
            //logger.Information( "link.RadioSetResponseDataMode result : " + result );

            Tag18K6CInventory(radioHandle);

            Tag18K6CRead(radioHandle);

            Tag18K6CQT(radioHandle);

            Tag18K6CBlockErase(radioHandle);

            Tag18K6CKill(radioHandle);

            Tag18K6CLock(radioHandle);

            RadioTurnCarrierWaveOnRandom(radioHandle);

            RegisterAccess(radioHandle);

            RadioClose(radioHandle);
        }

        private void Startup()
        {
            var version = new LibraryVersion();

            var result = link.Startup(version, 0);

            logger.Information("link.Startup result : {Result}", result);
            logger.Information("LibraryVersion {@Version}", new { version.major, version.minor, version.maintenance, version.release });
        }

        private void RegisterAccess(int radioHandle)
        {
            // test a banked register write            
            var result = link.MacWriteBankedRegister(radioHandle, 0x704, 2, 0xA5A5);
            logger.Information("link.MacWriteBankedRegister result : {Result}", result);

            // should give an error, due to invalid selector
            result = link.MacWriteBankedRegister(radioHandle, 0x704, 16, 0xA5A5);
            logger.Information("link.MacWriteBankedRegister result (Expect Invalid Parameter) : {Result}", result);


            // test a banked register read, last iteration i=16 should give an error
            UInt32 value = 0;
            for (UInt16 i = 0; i < 17; i++)
            {
                result = link.MacReadBankedRegister(radioHandle, 0x704, i, ref value);
                if (i < 16)
                {
                    logger.Information("link.MacReadBankedRegister result : {Result}", result);
                }
                else
                {
                    logger.Information("link.MacReadBankedRegister result (Expect Invalid Parameter) : {Result}", result);
                }
            }

            // Test OEMConfig Read/Write            
            UInt32[] readData = new UInt32[20];
            UInt32 numToRead = 16;
            result = link.MacReadOemData(radioHandle, 0x1f70, ref numToRead, readData);
            logger.Information("link.MacReadOemData result : " + result + ", numRead=" + numToRead);

            numToRead++;
            result = link.MacReadOemData(radioHandle, 0x1f70, ref numToRead, readData);
            logger.Information("link.MacReadOemData result (Expect Invalid Parameter) : " + result + ", numRead=" + numToRead);

            UInt32 numToWrite = 16;
            result = link.MacWriteOemData(radioHandle, 0x1f70, ref numToWrite, readData);
            logger.Information("link.MacWriteOemData result : " + result + ", numWritten=" + numToWrite);

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
            logger.Information("link.MacGetError (" + macError + ", " + macLastError + ") result : {Result}", result);

            // confirm a successful register access clears the mac error but not the lastMacError
            UInt32 macVer = 0;
            result = link.MacReadRegister(radioHandle, 0x0000, ref macVer);
            logger.Information("link.MacReadRegister result : {Result}", result);
            result = link.MacGetError(radioHandle, ref macError, ref macLastError);
            logger.Information("link.MacGetError (" + macError + ", " + macLastError + ") result : {Result}", result);
        }

        private void RadioTurnCarrierWaveOnRandom(int radioHandle)
        {
            var program = new Rfiddler();

            RandomCwParms randomCwParms = new RandomCwParms();

            // Enable Tx Random Data status packet by setting bits 7 and 0 in HST_CMNDIAGS (0x201)           
            UInt32 hstCmnDiags = 0;
            var result = link.MacReadRegister(radioHandle, 0x0201, ref hstCmnDiags);
            logger.Information("link.MacReadRegister result : {Result}", result);
            hstCmnDiags |= 0x81;
            result = link.MacWriteRegister(radioHandle, 0x0201, hstCmnDiags);
            logger.Information("link.MacWriteRegister result : {Result}", result);

            logger.Information("Starting random cw test:");
            logger.Information("Limiting to 40 MAC packets");

#if TEST_NULL_RANDOM_CW_CALLBACK
            randomCwParms.callback = null;
            randomCwParms.context = (IntPtr) null;
            randomCwParms.callbackCode = (IntPtr) null;
#else
            randomCwParms.callback = program.MyCallback;
            randomCwParms.context = IntPtr.Zero;
            randomCwParms.callbackCode = IntPtr.Zero;
#endif
            randomCwParms.duration = 10000;

            result = link.RadioTurnCarrierWaveOnRandom(radioHandle, randomCwParms);

            logger.Information("link.RadioTurnCarrierWaveOnRandom result : {Result}", result);
        }

        private void Tag18K6CLock(int radioHandle)
        {
            var program = new Rfiddler();

            // LOCK check - do not want to accidentally have tags locked unexpectedly...
            if (!program.enableTagLock)
            {
                return;
            }

            LockParms lockParms = new LockParms();

            lockParms.common.tagStopCount = 0;
            lockParms.common.callback = program.MyCallback;
            lockParms.common.context = IntPtr.Zero;
            lockParms.common.callbackCode = IntPtr.Zero;

            lockParms.lockCmdParms.permissions.killPasswordPermissions = PasswordPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.accessPasswordPermissions = PasswordPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.epcMemoryBankPermissions = MemoryPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.tidMemoryBankPermissions = MemoryPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.userMemoryBankPermissions = MemoryPermission.NO_CHANGE;

            lockParms.accessPassword = 0x0;

            var result = link.Tag18K6CLock(radioHandle, lockParms, 0);

            logger.Information("link.Tag18K6CLock result : {Result}", result);
        }

        private void Tag18K6CKill(int radioHandle)
        {
            var program = new Rfiddler();

            // KILL check - do not want to accidentally have tags killed unexpectedly...
            if (!program.enableTagKill)
            {
                return;
            }

            KillParms killParms = new KillParms();

            killParms.common.tagStopCount = 0;
            killParms.common.callback = program.MyCallback;
            killParms.common.context = IntPtr.Zero;
            killParms.common.callbackCode = IntPtr.Zero;

            killParms.accessPassword = 0x0;
            killParms.killCmdParms.killPassword = 0x0;

            var result = link.Tag18K6CKill(radioHandle, killParms, 0);

            logger.Information("link.Tag18K6CKill result : {Result}", result);
        }

        private void Tag18K6CBlockErase(int radioHandle)
        {
            var program = new Rfiddler();

            BlockEraseParms blockEraseParms = new BlockEraseParms();

            logger.Information("Starting basic block erase test:");
            logger.Information("Limiting of 40 MAC packets");

            blockEraseParms.common.tagStopCount = 0;
            blockEraseParms.common.callback = program.MyCallback;
            blockEraseParms.common.context = IntPtr.Zero;
            blockEraseParms.common.callbackCode = IntPtr.Zero;

            blockEraseParms.blockEraseCmdParms.bank = MemoryBank.RESERVED;
            blockEraseParms.blockEraseCmdParms.offset = 0;
            blockEraseParms.blockEraseCmdParms.count = 4;

            blockEraseParms.accessPassword = 0x0;

            var result = link.Tag18K6CBlockErase(radioHandle, blockEraseParms, 0);

            logger.Information("link.Tag18K6CBlockErase result : {Result}", result);
        }

        // ReSharper disable once InconsistentNaming
        private void Tag18K6CQT(int radioHandle)
        {
            var program = new Rfiddler();

            QTParms qtParms = new QTParms();


            logger.Information("Starting basic QT test:");
            logger.Information("Limiting of 40 MAC packets");

            qtParms.common.tagStopCount = 0;
            qtParms.common.callback = program.MyCallback;
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

            logger.Information("link.Tag18K6CQT result : {Result}", result);
        }

        private void Tag18K6CRead(int radioHandle)
        {
            Rfiddler program = new Rfiddler();

            ReadParms readParms = new ReadParms();

            logger.Information("Starting basic read test:");
            logger.Information("Limiting of 40 MAC packets");

            readParms.common.tagStopCount = 0;
            readParms.common.callback = program.MyCallback;
            readParms.common.context = IntPtr.Zero;
            readParms.common.callbackCode = IntPtr.Zero;

            readParms.readCmdParms.bank = MemoryBank.EPC;
            readParms.readCmdParms.offset = 2;
            readParms.readCmdParms.count = 6;

            readParms.accessPassword = 0x0;

            var result = link.Tag18K6CRead(radioHandle, readParms, 0);

            logger.Information("link.Tag18K6CRead result : {Result}", result);
        }

        private void Tag18K6CInventory(int radioHandle)
        {
            logger.Information("Starting basic inventory test:");
            logger.Information("Limiting of 40 MAC packets");


            Rfiddler program = new Rfiddler();

            InventoryParms inventoryParms = new InventoryParms();

            inventoryParms.common = new CommonParms();

            inventoryParms.common.tagStopCount = 0;
            inventoryParms.common.callback = program.MyCallback;
            inventoryParms.common.context = IntPtr.Zero;
            inventoryParms.common.callbackCode = IntPtr.Zero;

            var result = link.Tag18K6CInventory(radioHandle, inventoryParms, enableSelectCriteria | enablePostSingulationMatch);

            logger.Information("link.Tag18K6CInventory result : {Result}", result);
        }

        private void Set18K6CSingulationAlgorithmParameters(int radioHandle)
        {
            // Fixed.

            FixedQParms fqp = new FixedQParms();

            fqp.qValue = 1;
            fqp.retryCount = 1;
            fqp.repeatUntilNoTags = 1;
            if (1 == enableTagFocus)
            {
                fqp.toggleTarget = 0;
            }
            else
            {
                fqp.toggleTarget = 1;
            }

            var result = link.Set18K6CSingulationAlgorithmParameters(radioHandle, SingulationAlgorithm.FIXEDQ, fqp);

            logger.Information("link.Set18K6CSingulationAlgorithmParameters Fixed Q result : {Result}", result);

            // Dynamic.

            DynamicQParms dqp = new DynamicQParms();

            dqp.startQValue = 3;
            dqp.minQValue = 0;
            dqp.maxQValue = 7;
            dqp.retryCount = 1;
            dqp.thresholdMultiplier = 4;
            if (1 == enableTagSuppression)
            {
                dqp.toggleTarget = 0;
            }
            else
            {
                dqp.toggleTarget = 1;
            }

            result = link.Set18K6CSingulationAlgorithmParameters(radioHandle, SingulationAlgorithm.DYNAMICQ, dqp);

            logger.Information("link.Set18K6CSingulationAlgorithmParameters Dynamic Q result : {Result}", result);
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

            logger.Information("link.Set18K6CPostMatchCriteria result : {Result}", result);
        }

        private void UseImpinjExtensions(int radioHandle)
        {
            ImpinjExtensions extensions = new ImpinjExtensions();

            // Retrieve current settings
            var result = link.RadioGetImpinjExtensions(radioHandle, extensions);
            logger.Information("link.RadioGetImpinjExtensions result : {Result}", result);

            // now update based on the enables
            if (1 == enableTagFocus)
            {
                extensions.tagFocus = TagFocus.FOCUS_ENABLED;
            }
            else
            {
                extensions.tagFocus = TagFocus.FOCUS_DISABLED;
            }

            if (1 == enableFastId)
            {
                extensions.fastId = FastId.FAST_ID_ENABLED;
            }
            else
            {
                extensions.fastId = FastId.FAST_ID_DISABLED;
            }

            extensions.blockWriteMode = BlockWriteMode.AUTO;

            result = link.RadioSetImpinjExtensions(radioHandle, extensions);

            logger.Information("link.RadioSetImpinjExtensions result : {Result}", result);
        }

        private void Set18K6CQueryTagGroup(int radioHandle)
        {
            // Configure the query tag group to only get tags with the SL asserted
            TagGroup group = new TagGroup();
            group.target = SessionTarget.A;

            if (0 == enableSelectCriteria)
            {
                group.selected = Selected.SELECT_ALL;
            }
            else
            {
                group.selected = Selected.SELECT_ASSERTED;
            }

            if (1 == enableTagFocus)
            {
                group.session = Session.S1;
            }
            else
            {
                group.session = Session.S2;
            }

            var result = link.Set18K6CQueryTagGroup(radioHandle, group);

            logger.Information("link.Set18K6CQueryTagGroup result : {Result}", result);
        }

        private void Get18K6CSelectCriteria(int radioHandle, SelectCriteria selectCriteria)
        {
            Result result = link.Get18K6CSelectCriteria(radioHandle, selectCriteria);

            logger.Information("link.Get18K6CSelectCriteria result : {Result}", result);
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

            logger.Information("link.Set18K6CSelectCriteria result : {Result}", result);

            return selectCriteria;
        }

        private void AntennaPortSetConfiguration()
        {
            // TODO:  Add Set config for antenna port...
        }

        private void AntennaPortGetConfiguration(int radioHandle)
        {
            AntennaPortConfig antennaPortConfig = new AntennaPortConfig();

            var result = link.AntennaPortGetConfiguration(radioHandle, 0, antennaPortConfig);

            logger.Information("link.AntennaPortGetConfig result : {Result}", result);
            logger.Information("\tRadioHandle used              : " + radioHandle);
            logger.Information("\tLength found                  : " + antennaPortConfig.length);
            logger.Information("\tPowerLevel found              : " + antennaPortConfig.powerLevel);
            logger.Information("\tDwellTime found               : " + antennaPortConfig.dwellTime);
            logger.Information("\tNumberInventoryCycles found   : " + antennaPortConfig.numberInventoryCycles);
            logger.Information("\tphysicalRxPort found          : " + antennaPortConfig.physicalRxPort);
            logger.Information("\tPhysicalTxPort found          : " + antennaPortConfig.physicalTxPort);
            logger.Information("\tSense Threshold( glob ) found : " + antennaPortConfig.antennaSenseThreshold);
        }

        private void SetAntennaPortStatus(int radioHandle, AntennaPortState portState)
        {
            Result result = link.AntennaPortSetState(radioHandle, 0, portState);

            logger.Information("link.AntennaPortSetStatus ( {PortState} ) result : {Result}", portState, result);
        }

        private void GetAntennaPortStatus(int radioHandle)
        {
            AntennaPortStatus antennaPortStatus = new AntennaPortStatus();

            var result = link.AntennaPortGetStatus(radioHandle, 0, antennaPortStatus);

            logger.Information("link.AntennaPortGetStatus result : {Result}", result);
            logger.Information("\tRadioHandle used       : " + radioHandle);
            logger.Information("\tLength found           : " + antennaPortStatus.length);
            logger.Information("\tState found            : " + antennaPortStatus.state);
            logger.Information("\tSense Value found      : " + antennaPortStatus.antennaSenseValue);
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

                logger.Information("link.RadioSetCurrentLinkProfile ( " + currentLinkProfile + " ) result : {Result}", result);
                logger.Information("\tRadioHandle used           : " + radioHandle);


                result = link.RadioGetCurrentLinkProfile(radioHandle, ref currentLinkProfile);

                logger.Information("link.RadioGetCurrentLinkProfile result : {Result}", result);
                logger.Information("\tRadioHandle used           : " + radioHandle);
                logger.Information("\tCurrent Link Profile found : " + currentLinkProfile);
            }


            RadioLinkProfile linkProfile = new RadioLinkProfile();

            result = link.RadioGetLinkProfile(radioHandle, currentLinkProfile, linkProfile);

            logger.Information("link.RadioGetLinkProfile result : {Result}", result);
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

            logger.Information("link.RadioSetCurrentLinkProfile ( " + savedLinkProfile + " ) result : {Result}", result);
            logger.Information("\tRadioHandle used           : " + radioHandle);
        }

        private void SetRadioPowerState(int radioHandle, RadioPowerState powerState)
        {
            Result result = link.RadioSetPowerState(radioHandle, powerState);

            logger.Information("link.RadioSetPowerState ( {PowerState} ) result : {Result}", powerState, result);
        }

        private void RadioGetPowerState(int radioHandle)
        {
            var powerState = new RadioPowerState();
            var result = link.RadioGetPowerState(radioHandle, ref powerState);

            logger.Information("link.RadioGetPowerState result : {Result}", result);
            logger.Information("\tRadioHandle used    : " + radioHandle);
            logger.Information("\tPowerState found    : " + powerState);
        }

        private void RadioGetOperationMode(int radioHandle)
        {
            var operationMode = new RadioOperationMode();

            var result = link.RadioGetOperationMode(radioHandle, ref operationMode);

            logger.Information("link.RadioGetOperationMode result : {Result}", result);
            logger.Information("\tRadioHandle used    : " + radioHandle);
            logger.Information("\tOperationMode found : " + operationMode);
        }

        private void RadioSetOperationMode(int radioHandle, RadioOperationMode operationMode)
        {
            Result result = link.RadioSetOperationMode(radioHandle, operationMode);

            logger.Information("link.RadioSetOperationMode ( {OperationMode} ) result : {Result}", operationMode, result);
        }

        private void MacGetBootLoaderVersion(int radioHandle)
        {
            MacBootLoaderVersion blVersion = new MacBootLoaderVersion();
            var result = link.MacGetBootLoaderVersion(radioHandle, blVersion);

            logger.Information("link.MacGetBootLoaderVersion result : {Result}", result);
            logger.Information("MacVersion {@Version}", new { blVersion.major, blVersion.minor, blVersion.maintenance, blVersion.release });
        }

        private void GetMacVersion(int radioHandle)
        {
            MacVersion macVersion = new MacVersion();
            var result = link.MacGetVersion(radioHandle, macVersion);

            logger.Information("link.MacGetVersion result : {Result}", result);
            logger.Information("MacVersion {@Version}", new { macVersion.major, macVersion.minor, macVersion.maintenance, macVersion.release });
        }

        private int RadioOpen(RadioEnumeration radios)
        {
            var radioHandle = 0;

            Result result = link.RadioOpen(radios.radioInfo[0].cookie, ref radioHandle, MacMode.DEFAULT);

            logger.Information("link.RadioOpen result : {Result}", result);
            logger.Information("\tCookie used          : " + radios.radioInfo[0].cookie);
            logger.Information("\tRadioHandle obtained : " + radioHandle);

            return radioHandle;
        }

        private void RadioClose(int radioHandle)
        {
            Result result = link.RadioClose(radioHandle);

            logger.Information("link.RadioClose result : {Result}", result);
        }

        private RadioEnumeration RetrieveAttachedRadiosList()
        {
            RadioEnumeration re = new RadioEnumeration();

            Result result = link.RetrieveAttachedRadiosList(re, 0);

            logger.Information("link.RetrieveAttachedRadiosList result : {Result}", result);
            logger.Information("\tRadioEnum.length       : " + re.length);
            logger.Information("\tRadioEnum.totalLength  : " + re.totalLength);
            logger.Information("\tRadioEnum.countRadios  : " + re.countRadios);

            for (int index = 0; index < re.radioInfo.Length; ++index)
            {
                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.length : " + re.radioInfo[index].length);

                var version = re.radioInfo[index].driverVersion;
                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.driverVersion : {@Version}", new { version.major, version.minor, version.maintenance, version.release });

                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.cookie   : " + re.radioInfo[index].cookie);
                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.idLength : " + re.radioInfo[index].idLength);
                string uniqueId = "\tRadio " + index + " RadioEnum.radioInfo.uniqueId : ";

                int index2;

                for (index2 = 0; index2 < re.radioInfo[index].idLength; ++index2)
                {
                    uniqueId += (char) re.radioInfo[index].uniqueId[index2];
                }

                logger.Information(uniqueId);
            }

            return re;
        }

        private Int32 MyCallback([In] Int32 handle, [In] UInt32 bufferLength, [In] IntPtr pBuffer, [In, Out] IntPtr context)
        {
            Byte[] packetBuffer = new Byte[bufferLength];

            Marshal.Copy(pBuffer, packetBuffer, 0, (Int32)bufferLength);

            var packetFlags = packetBuffer[1];
            var packetType = (Int16)((packetBuffer[3] << 8) | packetBuffer[2]);

            var packetLength = (Int16)((packetBuffer[5] << 8) | packetBuffer[4]);
            var packetTypeString = "Mac Packet received, PacketType = ";
            packetTypeString += $"0x{packetType:X4}";

            logger.Information(packetTypeString);

            // if its an end packet, print the status string too.
            if (1 == packetType)
            {
                var packetStatusString = "EndPacket Status = ";
                packetStatusString += string.Format("0x{0:X2}{1:X2}{2:X2}{3:X2}", packetBuffer[15], packetBuffer[14], packetBuffer[13], packetBuffer[12]);

                logger.Information(packetStatusString);
            }
            else if (5 == packetType)
            {
                Int16 length = (Int16)(((packetLength - 3) * 4) - (packetFlags >> 6));
                string packetEpcString = "EPC = ";
                for (int index = 0; index < length; ++index)
                {
                    packetEpcString += $"{packetBuffer[20 + index]:X2}";
                }

                logger.Verbose(packetEpcString);
            }
            else if (6 == packetType) // access packet, print the flag word if non-zero, along with error indicators
            {
                Byte flagWord = packetBuffer[1];
                var packetFlagString = "AccessPacket Flag = ";
                packetFlagString += string.Format("0x{0:X2}", flagWord);

                if (0 != (flagWord & 0x03)) // some error occurred
                {
                    if (0 != (flagWord & 0x0E)) // backscatter error, reported in the tag_error_code
                    {
                        packetFlagString += ", Error = " + string.Format("0x{0:X2}", packetBuffer[13]);
                    }
                    else // MAC protocol access error, reported prot_error_code field
                    {
                        packetFlagString += ", Error = " + string.Format("0x{0:X2}{1:X2}", packetBuffer[15], packetBuffer[14]);
                    }
                    logger.Information(packetFlagString);
                }
            }

            ++_callbackCount;

            if (40 == _callbackCount)
            {
                return 1;
            }

            return 0;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            var result = link.Shutdown();

            logger.Information("link.Shutdown result : {Result}", result);

            disposed = true;
        }
    }
}