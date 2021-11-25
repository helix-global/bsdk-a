using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct CMSG_CMS_RECIPIENT_INFO32
        {
        [FieldOffset(0)] public readonly CMSG_CMS_RECIPIENT_INFO_CHOICE RecipientChoice;
        [FieldOffset(4)] public readonly unsafe CMSG_KEY_TRANS_RECIPIENT_INFO32* KeyTrans;
        [FieldOffset(4)] public readonly unsafe CMSG_KEY_AGREE_RECIPIENT_INFO32* KeyAgree;
        [FieldOffset(4)] public readonly unsafe CMSG_MAIL_LIST_RECIPIENT_INFO*   MailList;
        }
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct CMSG_CMS_RECIPIENT_INFO64
        {
        [FieldOffset(0)] public readonly CMSG_CMS_RECIPIENT_INFO_CHOICE RecipientChoice;
        [FieldOffset(8)] public readonly unsafe CMSG_KEY_TRANS_RECIPIENT_INFO64* KeyTrans;
        [FieldOffset(8)] public readonly unsafe CMSG_KEY_AGREE_RECIPIENT_INFO64* KeyAgree;
        [FieldOffset(8)] public readonly unsafe CMSG_MAIL_LIST_RECIPIENT_INFO*   MailList;
        }
    }