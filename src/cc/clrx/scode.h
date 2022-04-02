#pragma once
#include <string>

#define TRK_S_OUT_OF_SYNC                                           0x0DEAD100
#define TRK_VOLUME_NOT_FOUND                                        0x0DEAD102
#define TRK_VOLUME_NOT_OWNED                                        0x0DEAD103
#define TRK_S_NOTIFICATION_QUOTA_EXCEEDED                           0x0DEAD107
#define COR_E_AMBIGUOUSMATCH                                        0x8000211D
#define COR_E_INVALIDCAST                                           0x80004002
#define COR_E_NULLREFERENCE                                         0x80004003
#define COR_E_TARGETPARAMCOUNT                                      0x8002000E
#define COR_E_DIVIDEBYZERO                                          0x80020012
#define COR_E_FILENOTFOUND                                          0x80070002
#define COR_E_DIRECTORYNOTFOUND                                     0x80070003
#define XACT_E_CONNECTION_REQUEST_DENIED                            0x8004D100
#define XACT_E_TOOMANY_ENLISTMENTS                                  0x8004D101
#define XACT_E_DUPLICATE_GUID                                       0x8004D102
#define XACT_E_NOTSINGLEPHASE                                       0x8004D103
#define XACT_E_RECOVERYALREADYDONE                                  0x8004D104
#define XACT_E_PROTOCOL                                             0x8004D105
#define XACT_E_RM_FAILURE                                           0x8004D106
#define XACT_E_RECOVERY_FAILED                                      0x8004D107
#define XACT_E_LU_NOT_FOUND                                         0x8004D108
#define XACT_E_DUPLICATE_LU                                         0x8004D109
#define XACT_E_LU_NOT_CONNECTED                                     0x8004D10A
#define XACT_E_DUPLICATE_TRANSID                                    0x8004D10B
#define XACT_E_LU_BUSY                                              0x8004D10C
#define XACT_E_LU_NO_RECOVERY_PROCESS                               0x8004D10D
#define XACT_E_LU_DOWN                                              0x8004D10E
#define XACT_E_LU_RECOVERING                                        0x8004D10F
#define XACT_E_LU_RECOVERY_MISMATCH                                 0x8004D110
#define XACT_E_RM_UNAVAILABLE                                       0x8004D111
#define COR_E_UNAUTHORIZEDACCESS                                    0x80070005
#define COR_E_BADIMAGEFORMAT                                        0x8007000B
#define COR_E_OUTOFMEMORY                                           0x8007000E
#define COR_E_ENDOFSTREAM                                           0x80070026
#define COR_E_ARGUMENT                                              0x80070057
#define COR_E_PATHTOOLONG                                           0x800700CE
#define COR_E_ARITHMETIC                                            0x80070216
#define COR_E_STACKOVERFLOW                                         0x800703E9
#define VB6_SUBSCRIPT_OUT_OF_RANGE                                  0x800A0009
#define VB6_INPUT_PAST_END_OF_FILE                                  0x800A003E
#define VB6_COULD_NOT_ACCESS_SYSTEM_REGISTRY                        0x800A014F
#define VB6_PERMISSION_TO_USE_OBJECT_DENIED                         0x800A01A3
#define VB6_OBJECT_DOESNT_SUPPORT_THIS_PROPERTY_OR_METHOD           0x800A01B6
#define VB6_OBJECT_DOESNT_SUPPORT_THIS_ACTION                       0x800A01BD
#define VB6_ARGUMENT_NOT_OPTIONAL                                   0x800A01C1
#define VB6_WRONG_NUMBER_OF_ARGUMENTS                               0x800A01C2
#define VB6_VARIABLE_USES_A_TYPE_NOT_SUPPORTED                      0x800A01CA
#define VB6_THIS_COMPONENT_DOESNT_SUPPORT_THE_SET_OF_EVENTS         0x800A01CB
#define VB6_METHOD_OR_DATA_MEMBER_NOT_FOUND                         0x800A01CD
#define VB6_OUT_OF_MEMORY                                           0x800A7919
#define VB6_ERROR_SAVING_TO_FILE                                    0x800A793C
#define VB6_ERROR_LOADING_FROM_FILE                                 0x800A793D
#define COR_E_TYPEUNLOADED                                          0x80131013
#define COR_E_APPDOMAINUNLOADED                                     0x80131014
#define COR_E_CANNOTUNLOADAPPDOMAIN                                 0x80131015
#define COR_E_ASSEMBLYEXPECTED                                      0x80131018
#define COR_E_FIXUPSINEXE                                           0x80131019
#define COR_E_MODULE_HASH_CHECK_FAILED                              0x80131039
#define FUSION_E_REF_DEF_MISMATCH                                   0x80131040
#define FUSION_E_INVALID_PRIVATE_ASM_LOCATION                       0x80131041
#define FUSION_E_ASM_MODULE_MISSING                                 0x80131042
#define FUSION_E_SIGNATURE_CHECK_FAILED                             0x80131045
#define FUSION_E_INVALID_NAME                                       0x80131047
#define CLDB_E_FILE_OLDVER                                          0x80131107
#define CLDB_E_FILE_CORRUPT                                         0x8013110E
#define SECURITY_E_INCOMPATIBLE_SHARE                               0x80131401
#define SECURITY_E_UNVERIFIABLE                                     0x80131402
#define SECURITY_E_INCOMPATIBLE_EVIDENCE                            0x80131403
#define CORSEC_E_INVALID_STRONGNAME                                 0x80131415
#define CORSEC_E_POLICY_EXCEPTION                                   0x80131416
#define CORSEC_E_XMLSYNTAX                                          0x80131419
#define CORSEC_E_INVALID_IMAGE_FORMAT                               0x8013141D
#define CORSEC_E_CRYPTO                                             0x80131430
#define CORSEC_E_CRYPTO_UNEX_OPER                                   0x80131431
#define ISS_E_ISOSTORE                                              0x80131450
#define COR_E_EXCEPTION                                             0x80131500
#define COR_E_SYSTEM                                                0x80131501
#define COR_E_ARGUMENTOUTOFRANGE                                    0x80131502
#define COR_E_ARRAYTYPEMISMATCH                                     0x80131503
#define COR_E_CONTEXTMARSHAL                                        0x80131504
#define COR_E_EXECUTIONENGINE                                       0x80131506
#define COR_E_FIELDACCESS                                           0x80131507
#define COR_E_INDEXOUTOFRANGE                                       0x80131508
#define COR_E_INVALIDOPERATION                                      0x80131509
#define COR_E_SECURITY                                              0x8013150A
#define COR_E_REMOTING                                              0x8013150B
#define COR_E_SERIALIZATION                                         0x8013150C
#define COR_E_VERIFICATION                                          0x8013150D
#define COR_E_SERVER                                                0x8013150E
#define COR_E_METHODACCESS                                          0x80131510
#define COR_E_MISSINGFIELD                                          0x80131511
#define COR_E_MISSINGMEMBER                                         0x80131512
#define COR_E_MISSINGMETHOD                                         0x80131513
#define COR_E_MULTICASTNOTSUPPORTED                                 0x80131514
#define COR_E_NOTSUPPORTED                                          0x80131515
#define COR_E_OVERFLOW                                              0x80131516
#define COR_E_RANK                                                  0x80131517
#define COR_E_SYNCHRONIZATIONLOCK                                   0x80131518
#define COR_E_THREADINTERRUPTED                                     0x80131519
#define COR_E_MEMBERACCESS                                          0x8013151A
#define COR_E_THREADSTATE                                           0x80131520
#define COR_E_THREADSTOP                                            0x80131521
#define COR_E_TYPELOAD                                              0x80131522
#define COR_E_ENTRYPOINTNOTFOUND                                    0x80131523
#define COR_E_DLLNOTFOUND                                           0x80131524
#define COR_E_INVALIDCOMOBJECT                                      0x80131527
#define COR_E_NOTFINITENUMBER                                       0x80131528
#define COR_E_DUPLICATEWAITOBJECT                                   0x80131529
#define COR_E_THREADABORTED                                         0x80131530
#define COR_E_INVALIDOLEVARIANTTYPE                                 0x80131531
#define COR_E_MISSINGMANIFESTRESOURCE                               0x80131532
#define COR_E_SAFEARRAYTYPEMISMATCH                                 0x80131533
#define COR_E_TYPEINITIALIZATION                                    0x80131534
#define COR_E_MARSHALDIRECTIVE                                      0x80131535
#define COR_E_FORMAT                                                0x80131537
#define COR_E_SAFEARRAYRANKMISMATCH                                 0x80131538
#define COR_E_PLATFORMNOTSUPPORTED                                  0x80131539
#define COR_E_INVALIDPROGRAM                                        0x8013153A
#define COR_E_APPLICATION                                           0x80131600
#define COR_E_INVALIDFILTERCRITERIA                                 0x80131601
#define COR_E_REFLECTIONTYPELOAD                                    0x80131602
#define COR_E_TARGET                                                0x80131603
#define COR_E_TARGETINVOCATION                                      0x80131604
#define COR_E_CUSTOMATTRIBUTEFORMAT                                 0x80131605
#define COR_E_IO                                                    0x80131620
#define COR_E_FILELOAD                                              0x80131621
#define FVE_E_FAILED_BAD_FS                                         0x80310014
#define FWP_E_TOO_MANY_BOOTTIME_FILTERS                             0x80320018
#define FWP_E_INCOMPATIBLE_AUTH_CONFIG                              0x80320038
#define FWP_E_INCOMPATIBLE_CIPHER_CONFIG                            0x80320039
#define TRK_E_NOT_FOUND                                             0x8DEAD01B
#define TRK_E_VOLUME_QUOTA_EXCEEDED                                 0x8DEAD01C
#define TRK_SERVER_TOO_BUSY                                         0x8DEAD01E
#define ERROR_GRAPHICS_OPM_PARAMETER_ARRAY_TOO_SMALL                0xC0262504
#define ERROR_GRAPHICS_PVP_NO_DISPLAY_DEVICE_CORRESPONDS_TO_NAME    0xC0262506
#define ERROR_GRAPHICS_PVP_DISPLAY_DEVICE_NOT_ATTACHED_TO_DESKTOP   0xC0262507
#define ERROR_GRAPHICS_PVP_MIRRORING_DEVICES_NOT_SUPPORTED          0xC0262508
#define ERROR_GRAPHICS_OPM_INVALID_POINTER                          0xC026250A
#define ERROR_GRAPHICS_PVP_NO_MONITORS_CORRESPOND_TO_DISPLAY_DEVICE 0xC026250D
#define ERROR_GRAPHICS_PMEA_INVALID_MONITOR                         0xC02625D6
#define ERROR_GRAPHICS_PMEA_INVALID_D3D_DEVICE                      0xC02625D7

using namespace std;

class scode
    {
public:
    scode(const HRESULT value):
        value(value)
        {
        }
public:
    string str() const;
private:
    HRESULT value;
    };
