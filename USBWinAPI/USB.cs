using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using MS.Win32;

namespace USBWinAPI
{

    //Вспомогательная структура для получения всякой информации об устройстве
    struct device_info
    {
        public SP_DEVINFO_DATA dev_inst; //структура информации об устройстве
        public Guid dev_class;
        public info_disk info_disk; 
        public uint dev_number;
    };


    //Вспомогательная структура для информации о диске
    struct info_disk
    {
        public String  Dos_Device_Name;
        public String  Volume_Device_Name;
        public String  Volume_Name;
        public String  Bus_Type;
        public String  VendorId;
        public String  ProductId;
        public String  ProductRevision;
        public String  DeviceType;
        public String  DeviceNumber;
        public String  DevicePath;
        public String  Class;
        public String  Hardware_IDs;
        public String  Friendly_Name;
        public String  Physical_Device_Object_Name;
        public String  Device_Description;
        public String  Parent_Device_Instance_ID;
        public String  Parent_of_Parent_Device_Instance_ID;
        public String  DeviceInstanceId;
    };

    struct STORAGE_PROPERTY_QUERY {
      public STORAGE_PROPERTY_ID  PropertyId;
      public STORAGE_QUERY_TYPE  QueryType;
      public char AdditionalParameters;
    }; 

    struct STORAGE_DEVICE_DESCRIPTOR {
        public long Version;
        public long Size;
        public String DeviceType;
        public String DeviceTypeModifier;
        public Boolean RemovableMedia;
        public Boolean CommandQueueing;
        public long VendorIdOffset;
        public long ProductIdOffset;
        public long ProductRevisionOffset;
        public long SerialNumberOffset;
        public STORAGE_BUS_TYPE BusType;
        public long RawPropertiesLength;
        public String RawDeviceProperties;
        }

    //
    /*
     *standard header for information related to a device event 
     *dbch_Size - The size of this structure, in bytes.
     *dbch_devicetype - The device type, which determines the event-specific information
     *Если равно DBT_DEVTYP_HANDLE, то это не DEV_BROADCAST_HDR структура, а DEV_BROADCAST_HANDLE
     *DBT_DEVTYP_VOLUME - логический том
     *DBT_DEVTYP_OEM - OEM- or IHV-defined device type
     *DBT_DEVTYP_PORT - Port device (serial or parallel).
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DEV_BROADCAST_HDR
    {
        public uint dbch_Size;
        public uint dbch_DeviceType;
        public uint dbch_Reserved;
    }

    /*Расширенный заголовок - информация о файловой системе
     * dbch_devicetype - DBT_DEVTYP_HANDLE
     * dbch_handle - хендл для устройства
     * A handle to the device notification - возвращается функцией RegisterDeviceNotification
     * dbch_eventguid - The GUID for the custom event.  Valid only for DBT_CUSTOMEVENT.
     * dbch_nameoffset - The offset of an optional string buffer. Valid only for DBT_CUSTOMEVENT.     * 
     * dbch_data - Optional binary data. This member is valid only for DBT_CUSTOMEVENT.
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DEV_BROADCAST_HANDLE
    {
        public int dbch_size;
        public int dbch_devicetype;
        public int dbch_reserved;
        public IntPtr dbch_handle;
        public IntPtr dbch_hdevnotify;
        public Guid dbch_eventguid;
        public long dbch_nameoffset;
        public byte dbch_data;
        public byte dbch_data1;
    }


    /* An SP_DEVINFO_DATA structure defines a device 
     * instance that is a member of a device information set.
     * Структура для информации об устройстве
     * cbSize - размер структуры в байтах
     * classGuid - device's setup class.
     * Классы устройств:
     * @https://msdn.microsoft.com/en-us/library/windows/hardware/ff553426(v=vs.85).aspx
     * devInst - An opaque handle to the device instance
    */
    [StructLayout(LayoutKind.Sequential)]
    struct SP_DEVINFO_DATA
    {
        public int cbSize;
        public Guid classGuid;
        public uint devInst;
        public IntPtr reserved;
    }

    /// <summary>
    /// 
    /// </summary>
    enum STORAGE_PROPERTY_ID : uint
    {
      StorageDeviceProperty = 0,
      StorageAdapterProperty,
      StorageDeviceIdProperty
    }

    /// <summary>
    /// 
    /// </summary>
    enum STORAGE_QUERY_TYPE : uint
    {
      PropertyStandardQuery = 0, 
      PropertyExistsQuery, 
      PropertyMaskQuery, 
      PropertyQueryMaxDefined 
    }


    /*Determines whether the current window procedure 
     * is processing a message that was sent from another thread 
     * Флаги функции - InSendMessageEx - возвращаются этой функцией
     * ISMEX_NOSEND - не отправлен
     * ISMEX_SEND - The message was sent using the SendMessage or SendMessageTimeout function
     * ISMEX_NOTIFY - The message was sent using the SendNotifyMessage function
     * ISMEX_CALLBACK - The message was sent using the SendMessageCallback function
     * ISMEX_REPLIED - If ISMEX_REPLIED is not set, the thread that 
     * sent the message is blocked for ISMEX_SEND
    */
    [Flags] enum InSendMessageExFlags : uint
    {
        ISMEX_NOSEND   = 0,
        ISMEX_SEND     = 1,
        ISMEX_NOTIFY   = 2,
        ISMEX_CALLBACK = 4,
        ISMEX_REPLIED  = 8
    }

    /*Перечисление для того. чтобы указать будет ли показываться ошибка
     * SYSTEM_DEFAULT - по умолчанию
     */
    [Flags]
    public enum ErrorModes : uint
    {
        SYSTEM_DEFAULT = 0x0,
        SEM_FAILCRITICALERRORS = 0x0001,
        SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
        SEM_NOGPFAULTERRORBOX = 0x0002,
        SEM_NOOPENFILEERRORBOX = 0x8000
    }

    /*Определяет константы типа шины
    */
    enum STORAGE_BUS_TYPE : uint 
    { 
              BusTypeUnknown      = 0x00,
              BusTypeScsi         = 0x01,
              BusTypeAtapi        = 0x02,
              BusTypeAta          = 0x03,
              BusType1394         = 0x04,
              BusTypeSsa          = 0x05,
              BusTypeFibre        = 0x06,
              BusTypeUsb          = 0x07,
              BusTypeRAID         = 0x08,
              BusTypeiSCSI        = 0x09,
              BusTypeSas          = 0x0A,
              BusTypeSata         = 0x0B,
              BusTypeMaxReserved  = 0x7F
   }




    /*Определяет константы чтения, записи или чтения и записи файла.
     * Используется в конструкторе файлов
    */
    [Flags]
    enum EFileAccess : uint
    {
        //
        // Standart Section
        //

        AccessSystemSecurity = 0x1000000,   // AccessSystemAcl access type
        MaximumAllowed = 0x2000000,     // MaximumAllowed access type

        Delete = 0x10000,
        ReadControl = 0x20000,
        WriteDAC = 0x40000,
        WriteOwner = 0x80000,
        Synchronize = 0x100000,

        StandardRightsRequired = 0xF0000,
        StandardRightsRead = ReadControl,
        StandardRightsWrite = ReadControl,
        StandardRightsExecute = ReadControl,
        StandardRightsAll = 0x1F0000,
        SpecificRightsAll = 0xFFFF,

        FILE_READ_DATA = 0x0001,        // file & pipe
        FILE_LIST_DIRECTORY = 0x0001,       // directory
        FILE_WRITE_DATA = 0x0002,       // file & pipe
        FILE_ADD_FILE = 0x0002,         // directory
        FILE_APPEND_DATA = 0x0004,      // file
        FILE_ADD_SUBDIRECTORY = 0x0004,     // directory
        FILE_CREATE_PIPE_INSTANCE = 0x0004, // named pipe
        FILE_READ_EA = 0x0008,          // file & directory
        FILE_WRITE_EA = 0x0010,         // file & directory
        FILE_EXECUTE = 0x0020,          // file
        FILE_TRAVERSE = 0x0020,         // directory
        FILE_DELETE_CHILD = 0x0040,     // directory
        FILE_READ_ATTRIBUTES = 0x0080,      // all
        FILE_WRITE_ATTRIBUTES = 0x0100,     // all

        //
        // Generic Section
        //

        GenericRead = 0x80000000,
        GenericWrite = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000,

        SPECIFIC_RIGHTS_ALL = 0x00FFFF,
        FILE_ALL_ACCESS =
        StandardRightsRequired |
        Synchronize |
        0x1FF,

        FILE_GENERIC_READ =
        StandardRightsRead |
        FILE_READ_DATA |
        FILE_READ_ATTRIBUTES |
        FILE_READ_EA |
        Synchronize,

        FILE_GENERIC_WRITE =
        StandardRightsWrite |
        FILE_WRITE_DATA |
        FILE_WRITE_ATTRIBUTES |
        FILE_WRITE_EA |
        FILE_APPEND_DATA |
        Synchronize,

        FILE_GENERIC_EXECUTE =
        StandardRightsExecute |
          FILE_READ_ATTRIBUTES |
          FILE_EXECUTE |
          Synchronize
    }


    /* EFileShare
     * одержит константы, позволяющие управлять типом доступа, который другие 
     * объекты FileStream могут осуществлять к тому же файлу.
    */
    [Flags]
    public enum EFileShare : uint
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0x00000000,
        /// <summary>
        /// Enables subsequent open operations on an object to request read access. 
        /// Otherwise, other processes cannot open the object if they request read access. 
        /// If this flag is not specified, but the object has been opened for read access, the function fails.
        /// </summary>
        Read = 0x00000001,
        /// <summary>
        /// Enables subsequent open operations on an object to request write access. 
        /// Otherwise, other processes cannot open the object if they request write access. 
        /// If this flag is not specified, but the object has been opened for write access, the function fails.
        /// </summary>
        Write = 0x00000002,
        /// <summary>
        /// Enables subsequent open operations on an object to request delete access. 
        /// Otherwise, other processes cannot open the object if they request delete access.
        /// If this flag is not specified, but the object has been opened for delete access, the function fails.
        /// </summary>
        Delete = 0x00000004
    }

    /*
     * CreationDisposition
     * Содержит флаги для того, что делать с файлом при открытии
     * 
    */
    public enum ECreationDisposition : uint
    {
        /// <summary>
        /// Creates a new file. The function fails if a specified file exists.
        /// </summary>
        New = 1,
        /// <summary>
        /// Creates a new file, always. 
        /// If a file exists, the function overwrites the file, clears the existing attributes, combines the specified file attributes, 
        /// and flags with FILE_ATTRIBUTE_ARCHIVE, but does not set the security descriptor that the SECURITY_ATTRIBUTES structure specifies.
        /// </summary>
        CreateAlways = 2,
        /// <summary>
        /// Opens a file. The function fails if the file does not exist. 
        /// </summary>
        OpenExisting = 3,
        /// <summary>
        /// Opens a file, always. 
        /// If a file does not exist, the function creates a file as if dwCreationDisposition is CREATE_NEW.
        /// </summary>
        OpenAlways = 4,
        /// <summary>
        /// Opens a file and truncates it so that its size is 0 (zero) bytes. The function fails if the file does not exist.
        /// The calling process must open the file with the GENERIC_WRITE access right. 
        /// </summary>
        TruncateExisting = 5
    }



    /*
     * FileAttributes
     * Содержит флаги для атрибутов файла
    */
    [Flags]
    public enum EFileAttributes : uint
    {
        Readonly = 0x00000001,
        Hidden = 0x00000002,
        System = 0x00000004,
        Directory = 0x00000010,
        Archive = 0x00000020,
        Device = 0x00000040,
        Normal = 0x00000080,
        Temporary = 0x00000100,
        SparseFile = 0x00000200,
        ReparsePoint = 0x00000400,
        Compressed = 0x00000800,
        Offline = 0x00001000,
        NotContentIndexed = 0x00002000,
        Encrypted = 0x00004000,
        Write_Through = 0x80000000,
        Overlapped = 0x40000000,
        NoBuffering = 0x20000000,
        RandomAccess = 0x10000000,
        SequentialScan = 0x08000000,
        DeleteOnClose = 0x04000000,
        BackupSemantics = 0x02000000,
        PosixSemantics = 0x01000000,
        OpenReparsePoint = 0x00200000,
        OpenNoRecall = 0x00100000,
        FirstPipeInstance = 0x00080000
    }

    /*
     * Это перечисление содержит типы устройств
     * DRIVE_REMOVABLE - floppy или flash device
     * DRIVE_FIXED - hard drives
     * DRIVE_REMOTE - удаленное устройство
    */
    public enum DriveType : uint
    {
        /// <summary>The drive type cannot be determined.</summary>
        Unknown = 0,    //DRIVE_UNKNOWN
        /// <summary>The root path is invalid, for example, no volume is mounted at the path.</summary>
        Error = 1,        //DRIVE_NO_ROOT_DIR
        /// <summary>The drive is a type that has removable media, for example, a floppy drive or removable hard disk.</summary>
        Removable = 2,    //DRIVE_REMOVABLE
        /// <summary>The drive is a type that cannot be removed, for example, a fixed hard drive.</summary>
        Fixed = 3,        //DRIVE_FIXED
        /// <summary>The drive is a remote (network) drive.</summary>
        Remote = 4,        //DRIVE_REMOTE
        /// <summary>The drive is a CD-ROM drive.</summary>
        CDROM = 5,        //DRIVE_CDROM
        /// <summary>The drive is a RAM disk.</summary>
        RAMDisk = 6        //DRIVE_RAMDISK
    }


    // флаги для SetupDiGetClassDevs 
    [Flags]
    public enum DiGetClassFlags : uint
    {
        DIGCF_DEFAULT = 0x00000001,  // only valid with DIGCF_DEVICEINTERFACE
        DIGCF_PRESENT = 0x00000002,
        DIGCF_ALLCLASSES = 0x00000004,
        DIGCF_PROFILE = 0x00000008,
        DIGCF_DEVICEINTERFACE = 0x00000010,
    }



    //стили для создания окна
    [Flags]
    public enum ClassStyles : uint
    {
        /// <summary>Aligns the window's client area on a byte boundary (in the x direction). This style affects the width of the window and its horizontal placement on the display.</summary>
        ByteAlignClient = 0x1000,

        /// <summary>Aligns the window on a byte boundary (in the x direction). This style affects the width of the window and its horizontal placement on the display.</summary>
        ByteAlignWindow = 0x2000,

        /// <summary>
        /// Allocates one device context to be shared by all windows in the class.
        /// Because window classes are process specific, it is possible for multiple threads of an application to create a window of the same class.
        /// It is also possible for the threads to attempt to use the device context simultaneously. When this happens, the system allows only one thread to successfully finish its drawing operation.
        /// </summary>
        ClassDC = 0x40,

        /// <summary>Sends a double-click message to the window procedure when the user double-clicks the mouse while the cursor is within a window belonging to the class.</summary>
        DoubleClicks = 0x8,

        /// <summary>
        /// Enables the drop shadow effect on a window. The effect is turned on and off through SPI_SETDROPSHADOW.
        /// Typically, this is enabled for small, short-lived windows such as menus to emphasize their Z order relationship to other windows.
        /// </summary>
        DropShadow = 0x20000,

        /// <summary>Indicates that the window class is an application global class. For more information, see the "Application Global Classes" section of About Window Classes.</summary>
        GlobalClass = 0x4000,

        /// <summary>Redraws the entire window if a movement or size adjustment changes the width of the client area.</summary>
        HorizontalRedraw = 0x2,

        /// <summary>Disables Close on the window menu.</summary>
        NoClose = 0x200,

        /// <summary>Allocates a unique device context for each window in the class.</summary>
        OwnDC = 0x20,

        /// <summary>
        /// Sets the clipping rectangle of the child window to that of the parent window so that the child can draw on the parent.
        /// A window with the CS_PARENTDC style bit receives a regular device context from the system's cache of device contexts.
        /// It does not give the child the parent's device context or device context settings. Specifying CS_PARENTDC enhances an application's performance.
        /// </summary>
        ParentDC = 0x80,

        /// <summary>
        /// Saves, as a bitmap, the portion of the screen image obscured by a window of this class.
        /// When the window is removed, the system uses the saved bitmap to restore the screen image, including other windows that were obscured.
        /// Therefore, the system does not send WM_PAINT messages to windows that were obscured if the memory used by the bitmap has not been discarded and if other screen actions have not invalidated the stored image.
        /// This style is useful for small windows (for example, menus or dialog boxes) that are displayed briefly and then removed before other screen activity takes place.
        /// This style increases the time required to display the window, because the system must first allocate memory to store the bitmap.
        /// </summary>
        SaveBits = 0x800,

        /// <summary>Redraws the entire window if a movement or size adjustment changes the height of the client area.</summary>
        VerticalRedraw = 0x1
    }



    //стили для создания окна
    [Flags()]
    public enum WindowStyles : uint
    {
        /// <summary>The window has a thin-line border.</summary>
        WS_BORDER = 0x800000,

        /// <summary>The window has a title bar (includes the WS_BORDER style).</summary>
        WS_CAPTION = 0xc00000,

        /// <summary>The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.</summary>
        WS_CHILD = 0x40000000,

        /// <summary>Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.</summary>
        WS_CLIPCHILDREN = 0x2000000,

        /// <summary>
        /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated.
        /// If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
        /// </summary>
        WS_CLIPSIBLINGS = 0x4000000,

        /// <summary>The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.</summary>
        WS_DISABLED = 0x8000000,

        /// <summary>The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.</summary>
        WS_DLGFRAME = 0x400000,

        /// <summary>
        /// The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style.
        /// The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
        /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
        /// </summary>
        WS_GROUP = 0x20000,

        /// <summary>The window has a horizontal scroll bar.</summary>
        WS_HSCROLL = 0x100000,

        /// <summary>The window is initially maximized.</summary> 
        WS_MAXIMIZE = 0x1000000,

        /// <summary>The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary> 
        WS_MAXIMIZEBOX = 0x10000,

        /// <summary>The window is initially minimized.</summary>
        WS_MINIMIZE = 0x20000000,

        /// <summary>The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
        WS_MINIMIZEBOX = 0x20000,

        /// <summary>The window is an overlapped window. An overlapped window has a title bar and a border.</summary>
        WS_OVERLAPPED = 0x0,

        /// <summary>The window is an overlapped window.</summary>
        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

        /// <summary>The window is a pop-up window. This style cannot be used with the WS_CHILD style.</summary>
        WS_POPUP = 0x80000000u,

        /// <summary>The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.</summary>
        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

        /// <summary>The window has a sizing border.</summary>
        WS_SIZEFRAME = 0x40000,

        /// <summary>The window has a window menu on its title bar. The WS_CAPTION style must also be specified.</summary>
        WS_SYSMENU = 0x80000,

        /// <summary>
        /// The window is a control that can receive the keyboard focus when the user presses the TAB key.
        /// Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.  
        /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
        /// For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.
        /// </summary>
        WS_TABSTOP = 0x10000,

        /// <summary>The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.</summary>
        WS_VISIBLE = 0x10000000,

        /// <summary>The window has a vertical scroll bar.</summary>
        WS_VSCROLL = 0x200000
    }


    //Contains information about a device
    //DeviceType - The type of device. Values from 0 through 32,767 are reserved for use by Microsoft
    //DeviceNumber - The number of this device.
    //
    [StructLayout(LayoutKind.Sequential)]
    struct STORAGE_DEVICE_NUMBER
    {
        public int DeviceType;
        public int DeviceNumber;
        public int PartitionNumber;
    }



    //стили окна - для создания окна
    [Flags]
    public enum WindowStylesEx : uint
    {
        /// <summary>Specifies a window that accepts drag-drop files.</summary>
        WS_EX_ACCEPTFILES = 0x00000010,

        /// <summary>Forces a top-level window onto the taskbar when the window is visible.</summary>
        WS_EX_APPWINDOW = 0x00040000,

        /// <summary>Specifies a window that has a border with a sunken edge.</summary>
        WS_EX_CLIENTEDGE = 0x00000200,

        /// <summary>
        /// Specifies a window that paints all descendants in bottom-to-top painting order using double-buffering.
        /// This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. This style is not supported in Windows 2000.
        /// </summary>
        /// <remarks>
        /// With WS_EX_COMPOSITED set, all descendants of a window get bottom-to-top painting order using double-buffering.
        /// Bottom-to-top painting order allows a descendent window to have translucency (alpha) and transparency (color-key) effects,
        /// but only if the descendent window also has the WS_EX_TRANSPARENT bit set.
        /// Double-buffering allows the window and its descendents to be painted without flicker.
        /// </remarks>
        WS_EX_COMPOSITED = 0x02000000,

        /// <summary>
        /// Specifies a window that includes a question mark in the title bar. When the user clicks the question mark,
        /// the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message.
        /// The child window should pass the message to the parent window procedure, which should call the WinHelp function using the HELP_WM_HELP command.
        /// The Help application displays a pop-up window that typically contains help for the child window.
        /// WS_EX_CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.
        /// </summary>
        WS_EX_CONTEXTHELP = 0x00000400,

        /// <summary>
        /// Specifies a window which contains child windows that should take part in dialog box navigation.
        /// If this style is specified, the dialog manager recurses into children of this window when performing navigation operations
        /// such as handling the TAB key, an arrow key, or a keyboard mnemonic.
        /// </summary>
        WS_EX_CONTROLPARENT = 0x00010000,

        /// <summary>Specifies a window that has a double border.</summary>
        WS_EX_DLGMODALFRAME = 0x00000001,

        /// <summary>
        /// Specifies a window that is a layered window.
        /// This cannot be used for child windows or if the window has a class style of either CS_OWNDC or CS_CLASSDC.
        /// </summary>
        WS_EX_LAYERED = 0x00080000,

        /// <summary>
        /// Specifies a window with the horizontal origin on the right edge. Increasing horizontal values advance to the left.
        /// The shell language must support reading-order alignment for this to take effect.
        /// </summary>
        WS_EX_LAYOUTRTL = 0x00400000,

        /// <summary>Specifies a window that has generic left-aligned properties. This is the default.</summary>
        WS_EX_LEFT = 0x00000000,

        /// <summary>
        /// Specifies a window with the vertical scroll bar (if present) to the left of the client area.
        /// The shell language must support reading-order alignment for this to take effect.
        /// </summary>
        WS_EX_LEFTSCROLLBAR = 0x00004000,

        /// <summary>
        /// Specifies a window that displays text using left-to-right reading-order properties. This is the default.
        /// </summary>
        WS_EX_LTRREADING = 0x00000000,

        /// <summary>
        /// Specifies a multiple-document interface (MDI) child window.
        /// </summary>
        WS_EX_MDICHILD = 0x00000040,

        /// <summary>
        /// Specifies a top-level window created with this style does not become the foreground window when the user clicks it.
        /// The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
        /// The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the WS_EX_APPWINDOW style.
        /// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
        /// </summary>
        WS_EX_NOACTIVATE = 0x08000000,

        /// <summary>
        /// Specifies a window which does not pass its window layout to its child windows.
        /// </summary>
        WS_EX_NOINHERITLAYOUT = 0x00100000,

        /// <summary>
        /// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.
        /// </summary>
        WS_EX_NOPARENTNOTIFY = 0x00000004,

        /// <summary>Specifies an overlapped window.</summary>
        WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,

        /// <summary>Specifies a palette window, which is a modeless dialog box that presents an array of commands.</summary>
        WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,

        /// <summary>
        /// Specifies a window that has generic "right-aligned" properties. This depends on the window class.
        /// The shell language must support reading-order alignment for this to take effect.
        /// Using the WS_EX_RIGHT style has the same effect as using the SS_RIGHT (static), ES_RIGHT (edit), and BS_RIGHT/BS_RIGHTBUTTON (button) control styles.
        /// </summary>
        WS_EX_RIGHT = 0x00001000,

        /// <summary>Specifies a window with the vertical scroll bar (if present) to the right of the client area. This is the default.</summary>
        WS_EX_RIGHTSCROLLBAR = 0x00000000,

        /// <summary>
        /// Specifies a window that displays text using right-to-left reading-order properties.
        /// The shell language must support reading-order alignment for this to take effect.
        /// </summary>
        WS_EX_RTLREADING = 0x00002000,

        /// <summary>Specifies a window with a three-dimensional border style intended to be used for items that do not accept user input.</summary>
        WS_EX_STATICEDGE = 0x00020000,

        /// <summary>
        /// Specifies a window that is intended to be used as a floating toolbar.
        /// A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font.
        /// A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB.
        /// If a tool window has a system menu, its icon is not displayed on the title bar.
        /// However, you can display the system menu by right-clicking or by typing ALT+SPACE. 
        /// </summary>
        WS_EX_TOOLWINDOW = 0x00000080,

        /// <summary>
        /// Specifies a window that should be placed above all non-topmost windows and should stay above them, even when the window is deactivated.
        /// To add or remove this style, use the SetWindowPos function.
        /// </summary>
        WS_EX_TOPMOST = 0x00000008,

        /// <summary>
        /// Specifies a window that should not be painted until siblings beneath the window (that were created by the same thread) have been painted.
        /// The window appears transparent because the bits of underlying sibling windows have already been painted.
        /// To achieve transparency without these restrictions, use the SetWindowRgn function.
        /// </summary>
        WS_EX_TRANSPARENT = 0x00000020,

        /// <summary>Specifies a window that has a border with a raised edge.</summary>
        WS_EX_WINDOWEDGE = 0x00000100
    }


    //информация об интерфейсе устройства
    /*interfaceClassGuid - The GUID for the class to which the device interface belongs 
    */
    [StructLayout(LayoutKind.Sequential)]
    struct SP_DEVICE_INTERFACE_DATA
    {
        public Int32 cbSize;
        public Guid interfaceClassGuid; 
        public Int32 flags;
        private UIntPtr reserved;
    }


    //информация об интерфейсе устройства - детализированная - contains the path for a device interface
    /*
     * DevicePath - A NULL-terminated string that contains the device interface path. 
     * This path can be passed to Win32 functions such as CreateFile. 
    */
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct SP_DEVICE_INTERFACE_DETAIL_DATA
    {
        public int cbSize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string DevicePath;
    }

    //повторяет структуру SP_DEVICE_INTERFACE_DETAIL_DATA
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    struct NativeDeviceInterfaceDetailData
    {
        public int size;
        public char devicePath;
    }


    /*
     * WNDCLASSEX - структура для создания окна
     * 
    */
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WNDCLASSEX
    {

        [MarshalAs(UnmanagedType.U4)]
        public int cbSize;
        [MarshalAs(UnmanagedType.U4)]
        public ClassStyles style;
        public IntPtr lpfnWndProc; // not WndProc
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;

        //Use this function to make a new one with cbSize already filled in.
        //For example:
        //var WndClss = WNDCLASSEX.Build()
        public static WNDCLASSEX Build()
        {
            var nw = new WNDCLASSEX();
            nw.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            return nw;
        }
    }


    


    class USB
    {

        //=================================================================
        //Константы WINAPI
        //=================================================================

        private const int  WM_DEVICECHANGE                           = 0x0219;
        private const int  DBT_DEVICEARRIVAL                         = 0x8000;  // system detected a new device
        private const int  DBT_DEVTYP_VOLUME                         = 0x00000002;  // logical volume
        private const int  DBT_CONFIGCHANGECANCELED                  = 0x0019;
        private const int  DBT_CONFIGCHANGED                         = 0x0018;
        private const int  DBT_CUSTOMEVENT                           = 0x8006;
        private const int  DBT_DEVICEQUERYREMOVE                     = 0x8001;
        private const int  DBT_DEVICEQUERYREMOVEFAILED               = 0x8002;
        private const int  DBT_DEVICEREMOVECOMPLETE                  = 0x8004;
        private const int  DBT_DEVICEREMOVEPENDING                   = 0x8003;
        private const int  DBT_DEVICETYPESPECIFIC                    = 0x8005;
        private const int  DBT_DEVNODES_CHANGED                      = 0x0007;
        private const int  DBT_QUERYCHANGECONFIG                     = 0x0017;
        private const int  DBT_USERDEFINED                           = 0xFFFF;
        private const int  DBT_DEVTYP_HANDLE                         = 0x0006;
        private const int  CR_SUCCESS                                = 0x0000;
        private const int  BROADCAST_QUERY_DENY                      = 0x424D5144;
        private const int  ISMEX_REPLIED                             = 8;
        private const int  SEM_FAILCRITICALERRORS                    = 0x0001;
        private const int  INVALID_HANDLE_VALUE                      = -1;
        private const int  DEVICE_NOTIFY_WINDOW_HANDLE               = 0;
        private const int  MAX_PATH                                  = 260; //максимальные путь
        private const int  CM_REMOVAL_POLICY_EXPECT_NO_REMOVAL       = 1;
        private const int  CM_REMOVAL_POLICY_EXPECT_ORDERLY_REMOVAL  = 2;
        private const int  CM_REMOVAL_POLICY_EXPECT_SURPRISE_REMOVAL = 3;
        private const int  SPDRP_REMOVAL_POLICY                      = 0x001F;
        private const int  IOCTL_STORAGE_GET_DEVICE_NUMBER           = 0x2D1080;
        private const int  IOCTL_STORAGE_QUERY_PROPERTY              = 0x2D1400;
        private const int  CW_USEDEFAULT                             = 0x8000;
        private const int  GWLP_USERDATA                             = -21;

        //=================================================================


        //=================================================================


        //Переменные для хранения состояния и хендла окна
        //и настройки мониторинга внешних жестких дисков
        private bool started;
        private bool safe_remove_on_timeout;
        private bool monitor_hard_drives;

        //хендл окна
        private IntPtr mon_hwnd;

        //Список существующих в данный момент USB-флешек и дисков
        private List<char> existingUSB;

        //зарегистрированнные подписки на флешки
        //private Dictionary<IntPtr, Tuple<IntPtr, char>> notifications;
        private Dictionary<IntPtr, Tuple<IntPtr, char>> exist_notifications 
            = new Dictionary<IntPtr,Tuple<IntPtr,char>>();      

        //=================================================================


        //=================================================================

        //конструктор
        //избежать неявного преобразования
        private USB()
        {           
            //состояние работы
            started = false;
            //безопасное извлечение при тайм-ауте
            safe_remove_on_timeout = true;
            //монитор жестких дисков
            monitor_hard_drives = false;
            //Получаем и сохраняем список всех существующих USB-девайсов
            existingUSB = get_flash_disks(monitor_hard_drives);
          
        }


        //синглтон при помощи лямбда преобразований и лейзи функции
        private static readonly Lazy<USB> lazy =
        new Lazy<USB>(() => new USB());

        /// <summary>
        /// инстанс класса
        /// </summary>
        public static USB Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        //=================================================================


        //=================================================================

        /// <summary>
        /// делегат WndProc - для создания окна может пригодиться через CreateWindowEx
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private delegate IntPtr WndProcDelegate(IntPtr hwnd, int msg, IntPtr wParam,
            IntPtr lParam, ref bool handled);



        /// <summary>
        /// Changes an attribute of the specified window. 
        /// The function also sets a value at the specified offset in the extra window memory.
        /// Меняет атрибуты выбранного окна
        /// nIndex устанавливается в GWLP_USERDATA Sets the user data associated with the window
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex">nIndex - The zero-based offset to the value to be set</param>
        /// <param name="dwNewLong"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);



        



        /// <summary>
        /// Unregisters a window class, freeing the memory required for the class.
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="hInstance"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dnDevInst"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferLen"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static extern int CM_Get_Device_ID(
           UInt32 dnDevInst,
           IntPtr buffer,
           int bufferLen,
           int flags
        );


        /// <summary>
        /// Функция для создания окна
        /// </summary>
        /// <param name="dwExStyle"></param>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <param name="dwStyle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="hWndParent"></param>
        /// <param name="hMenu"></param>
        /// <param name="hInstance"></param>
        /// <param name="lpParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
           WindowStylesEx dwExStyle,
           string lpClassName,
           string lpWindowName,
           WindowStyles dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);


        /// <summary>
        /// функция для регистрации в памяти окна
        /// </summary>
        /// <param name="lpwcx"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.U2)]
        static extern short RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        
        /// <summary>
        /// Retrieves a module handle for the specified module. 
        /// The module must have been loaded by the calling process.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        [DllImport("coredll.dll")]
        static extern IntPtr GetModuleHandle(string module);


        /// <summary>
        /// функция вызова оконной процедуры принимает делегат функции
        /// </summary>
        /// <param name="lpPrevWndFunc"></param>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallWindowProc(WndProcDelegate lpPrevWndFunc,
            IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);


        /// <summary>
        /// функции показывающая отправлено ли сообщение
        /// </summary>
        /// <param name="lpReserved"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern InSendMessageExFlags InSendMessageEx(IntPtr lpReserved);



        /// <summary>
        /// освобождает память для записи об устройства
        /// </summary>
        /// <param name="Handle"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterDeviceNotification(IntPtr Handle);


        /// <summary>
        /// закрывает хендл
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);


        /// <summary>
        /// The CM_Request_Device_Eject function prepares a local device 
        /// instance for safe removal, if the device is removable. 
        /// If the device can be physically ejected, it will be.
        /// Приготавливает устройство для безопасного извлечения
        /// </summary>
        /// <param name="dnDevInst"></param>
        /// <param name="pVetoType"></param>
        /// <param name="pszVetoName"></param>
        /// <param name="ulNameLength"></param>
        /// <param name="ulFlags"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern int CM_Request_Device_EjectW(SP_DEVINFO_DATA dnDevInst, ref int pVetoType,
            StringBuilder pszVetoName, int ulNameLength, int ulFlags);


        /// <summary>
        /// установить текущий вывод ошибок
        /// </summary>
        /// <param name="uMode"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern ErrorModes SetErrorMode(ErrorModes uMode);



        /// <summary>
        /// Creates or opens a file or I/O device. 
        /// The most commonly used I/O devices are as follows: 
        /// file, file stream, directory, physical disk, volume, 
        /// console buffer, tape drive, communications resource, mailslot, and pipe        
        /// EFileAccess, EFileShare, ECreationDisposition, EFileAttributes - флаги перечисленные выше  
        /// </summary>
        /// <param name="filename">The name of the file or device to be created or opened.</param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <param name="securityAttributes"></param>
        /// <param name="creationDisposition"></param>
        /// <param name="flagsAndAttributes"></param>
        /// <param name="templateFile"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr CreateFileW(
             [MarshalAs(UnmanagedType.LPWStr)] string filename,
             [MarshalAs(UnmanagedType.U4)] EFileAccess access,
             [MarshalAs(UnmanagedType.U4)] EFileShare share,
             IntPtr securityAttributes,
             [MarshalAs(UnmanagedType.U4)] ECreationDisposition creationDisposition,
             [MarshalAs(UnmanagedType.U4)] EFileAttributes flagsAndAttributes,
             IntPtr templateFile);


        /// <summary>
        /// регистрировать устройство на окне, чтобы оно отправляло свои сообщения окну
        /// </summary>
        /// <param name="hRecipient"></param>
        /// <param name="NotificationFilter"></param>
        /// <param name="Flags"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient,
           IntPtr NotificationFilter, uint Flags);


        /// <summary>
        /// возвращает существующие логические устройства
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern uint GetLogicalDrives();


        
        /// <summary>
        /// получить тип устройства
        /// </summary>
        /// <param name="lpRootPathName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern DriveType GetDriveType([MarshalAs(UnmanagedType.LPStr)] 
            string lpRootPathName);


        /// <summary>
        /// Retrieves information about MS-DOS device names
        /// </summary>
        /// <param name="lpDeviceName"></param>
        /// <param name="lpTargetPath"></param>
        /// <param name="ucchMax"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern int QueryDosDeviceW(string lpDeviceName, IntPtr lpTargetPath,
           int ucchMax);


        /// <summary>
        /// The SetupDiGetClassDevs function returns a handle 
        /// to a device information set that contains requested 
        /// device information elements for a local computer.
        /// </summary>
        /// <param name="ClassGuid"></param>
        /// <param name="Enumerator"></param>
        /// <param name="hwndParent"></param>
        /// <param name="Flags"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid,
                                              IntPtr Enumerator, 
                                              IntPtr hwndParent,
                                              DiGetClassFlags Flags
                                             );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeviceInfoSet"></param>
        /// <param name="MemberIndex"></param>
        /// <param name="DeviceInfoData"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex,
            ref SP_DEVINFO_DATA DeviceInfoData);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeviceInfoSet"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceInfoSet"></param>
        /// <param name="deviceInfoData"></param>
        /// <param name="property"></param>
        /// <param name="propertyRegDataType"></param>
        /// <param name="propertyBuffer"></param>
        /// <param name="propertyBufferSize"></param>
        /// <param name="requiredSize"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(
            IntPtr deviceInfoSet,
            ref SP_DEVINFO_DATA deviceInfoData,
            uint property,
            IntPtr propertyRegDataType,
            byte[] propertyBuffer,
            uint propertyBufferSize,
            IntPtr requiredSize
            );



        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="dwIoControlCode"></param>
        /// <param name="lpInBuffer"></param>
        /// <param name="nInBufferSize"></param>
        /// <param name="lpOutBuffer"></param>
        /// <param name="nOutBufferSize"></param>
        /// <param name="lpBytesReturned"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
        internal static extern int DeviceIoControl(
            int hDevice,
            int dwIoControlCode,
            byte[] lpInBuffer,
            int nInBufferSize,
            ref STORAGE_DEVICE_NUMBER lpOutBuffer,
            int nOutBufferSize,
            ref int lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
        internal static extern int DeviceIoControl(
            int hDevice,
            int dwIoControlCode,
            ref STORAGE_PROPERTY_QUERY lpInBuffer,
            int nInBufferSize,
            ref STORAGE_DEVICE_DESCRIPTOR lpOutBuffer,
            int nOutBufferSize,
            ref int lpBytesReturned,
            IntPtr lpOverlapped);



        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDevInfo"></param>
        /// <param name="devInfo"></param>
        /// <param name="interfaceClassGuid"></param>
        /// <param name="memberIndex"></param>
        /// <param name="deviceInterfaceData"></param>
        /// <returns></returns>
        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(
           IntPtr hDevInfo,
           IntPtr devInfo,
           ref Guid interfaceClassGuid,
           UInt32 memberIndex,
           ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
        );



        //получить детальную информацию об устройства - вызывается два раза - первый раз
        //для получения длины структуры, второй раз для получения самой структуры
        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(
           IntPtr hDevInfo,
           ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
           ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
           UInt32 deviceInterfaceDetailDataSize,
           out UInt32 requiredSize,
           ref SP_DEVINFO_DATA deviceInfoData
        );


        //получить детальную информацию об устройства - вызывается два раза - первый раз
        //для получения длины структуры, второй раз для получения самой структуры
        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(
           IntPtr hDevInfo,
           ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
           IntPtr deviceInterfaceDetailData,
           UInt32 deviceInterfaceDetailDataSize,
           out UInt32 requiredSize,
           IntPtr deviceInfoData
        );


        //получить родительский элемент для устройства, 
        //например для флешки - ее хаб
        [DllImport("setupapi.dll")]
        static extern int CM_Get_Parent(
           out uint pdnDevInst,
           uint dnDevInst,
           int ulFlags
        );


        /// <summary>
        /// Получение имени see http://msdn.microsoft.com/en-us/library/cc542456.aspx
        /// how to find Volume name: \\?\Volume{4c1b02c1-d990-11dc-99ae-806e6f6e6963}\
        /// for the Paths:  C:\
        /// or device name like \Device\HarddiskVolume2 or \Device\CdRom0
        /// </summary>
        /// <param name="lpDeviceName"></param>
        /// <param name="lpTargetPath"></param>
        /// <param name="ucchMax"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        static extern uint QueryDosDevice(string lpDeviceName, IntPtr lpTargetPath,
           int ucchMax);



        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        //=================================================================


        //=================================================================

        //WndProc - процедура заменяющая окно - принимает сообщение
        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam,
            IntPtr lParam, ref bool handled)
        {

            switch (msg)
            {
                //Если это то, что нам нужно
                case WM_DEVICECHANGE:
                {

                    if (wParam == IntPtr.Zero)
                        throw new Exception("Found it");

                    //IntPtr data = GetWindowLongPtr(hwnd, GWLP_USERDATA);
                    //if (data != IntPtr.Zero)
                    return devices_changed(wParam, lParam);
                }
                //break;
            }
            return IntPtr.Zero;

        }


        //Функция, которая вызывается для обработки сообщения WM_DEVICECHANGE
        private IntPtr devices_changed(IntPtr wParam, IntPtr lParam)
        {


            //если включен мониторинг, то происходит обработка сообщений виндоус
            if (started)
            {

                //установка структуры DEV_BROADCAST_HDR
                DEV_BROADCAST_HDR pHdr = new DEV_BROADCAST_HDR();
                if (lParam != IntPtr.Zero)
                {
                    pHdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure
                        (lParam, typeof(DEV_BROADCAST_HDR));
                }


                //0x7 - DBT_DEVNODES_CHANGED
                int change = wParam.ToInt32();
                switch (wParam.ToInt32())
                {
                        //Если вставили устройство
                        case DBT_DEVICEARRIVAL:
                            //И если это дисковое устройство, 
                            if (pHdr.dbch_DeviceType == DBT_DEVTYP_VOLUME)
                            {
                                //то проверим
                                //изменения в буквах дисков, интересующих нас
                                detect_changed_devices();
                            }
                            break;



                        //Если какое-то устройство не удалось безопасно извлечь
                        case DBT_DEVICEQUERYREMOVEFAILED:
                            //И это хендл то структура возвращается
                            if (pHdr.dbch_DeviceType == DBT_DEVTYP_HANDLE)
                            {

                                DEV_BROADCAST_HANDLE pHand = (DEV_BROADCAST_HANDLE)Marshal.PtrToStructure
                                (lParam, typeof(DEV_BROADCAST_HANDLE));

                                //если в существующих подсоединениях есть хендл
                                if (exist_notifications.ContainsKey(pHand.dbch_handle))
                                {
                                    //если делегат содержит функцию
                                    //то вызвать ее со значением имени диска
                                    if (on_device_adds.GetInvocationList().GetLength(0) != 0)
                                    {
                                        //получение буквы диска
                                        char c = exist_notifications[pHand.dbch_handle].Item2;
                                        //вызов функции делегата
                                        on_device_adds(c);
                                    }

                                }

                            }
                        break;


                        //Если пришел запрос на безопасное извлечение устройства
                        case DBT_DEVICEQUERYREMOVE:
                            //И это хендл
                            if (pHdr.dbch_DeviceType == DBT_DEVTYP_HANDLE)
                            {

                                DEV_BROADCAST_HANDLE pHand = (DEV_BROADCAST_HANDLE)Marshal.PtrToStructure
                                (lParam, typeof(DEV_BROADCAST_HANDLE));

                                //если в существующих подсоединениях есть хендл
                                if (exist_notifications.ContainsKey(pHand.dbch_handle))
                                {
                                    //можем ли мы разрешить
                                    //извлекать это устройство
                                    if (on_device_safe_removes.GetInvocationList().GetLength(0) != 0)
                                    {
                                        //Если нет - вернем системе код отказа
                                        if (!on_device_safe_removes(exist_notifications[pHand.dbch_handle].Item2))
                                            //return BROADCAST_QUERY_DENY;
                                            return IntPtr.Zero;
                                        //Пользователь мог вызвать safe_eject внутри on_device_safe_removed, поэтому
                                        //проверим этот момент еще раз
                                    }

                                    //Если делегат не был задан, или программа разрешила извлечение
                                    //и при этом девайс не был извлечен принудительно
                                    if (exist_notifications.ContainsKey(pHand.dbch_handle))
                                    {

                                        //Выясним, а не прошел ли уже таймаут ожидания системой
                                        //ответа на ивент DBT_DEVICEQUERYREMOVE
                                        if (safe_remove_on_timeout && (InSendMessageEx(IntPtr.Zero) == InSendMessageExFlags.ISMEX_REPLIED))
                                        {
                                            //Если прошел и задана опция извлечения после таймаута,
                                            //принудительно извлечем устройство
                                            try
                                            {
                                                safe_eject(exist_notifications[pHand.dbch_handle].Item2);
                                            }
                                            catch(Exception ex)
                                            {
                                                //Ничего не делаем, так как устройство
                                                //может быть занято кем-то еще
                                            }
                                        }
                                        else
                                        {
                                            //Если таймаут не вышел, то освобождаем устройство
                                            //и разрешаем его извлечь (return TRUE в самом низу)
                                            UnregisterDeviceNotification(exist_notifications[pHand.dbch_handle].Item1);
                                            CloseHandle(pHand.dbch_handle);
                                            exist_notifications.Remove(pHand.dbch_handle);
                                        }
                                    }

                                }
                            }
                        break;

                        //Если какое-то устройство извлечено
                        //(небезопасно, например)
                        case DBT_DEVICEREMOVECOMPLETE:
                            //И это дисковое устройство, проверим изменения в интересующих нас буквах дисков
                            if (pHdr.dbch_DeviceType == DBT_DEVTYP_VOLUME)
                            {
                                detect_changed_devices();
                            }
                        break;
                }

            }
            return IntPtr.Zero;
        }


        //=================================================================


        //=================================================================

        //определяет, какие с момента последнего ее вызова устройства были добавлены и удалены
        private void detect_changed_devices()
        {

            //Список вставленных и вытащенных с последнего вызова функции устройств
            List<char> inserted, ejected;

            //Получаем текущий список интересующих нас устройств
            List<char> new_device_list = get_flash_disks(monitor_hard_drives);
            
            //какие буквы дисков добавились,
            //а какие были удалены
            inserted = new_device_list.Except(existingUSB).ToList();
            ejected = existingUSB.Except(new_device_list).ToList();

            //Сохраняем новый список устройств
            existingUSB = new_device_list;

            //Берем под контроль вставленные устройства
            foreach (char ins in inserted)
            {
                mount_device(ins);
            }

            //И отпускаем извлеченные (в этом месте те устройства, которые были извлечены
            //безопасно, уже освобождены нами)
            foreach (char ins in ejected)
            {
                unmount_device(ins, true);
            }


        }

        //берут устройство под контроль класса 
        private void mount_device(char letter)
        {

            //Проверяем, не подконтрольно ли нам уже это устройство
            foreach (KeyValuePair<IntPtr, Tuple<IntPtr, char>> note in exist_notifications)
            {
                if (note.Value.Item2 == letter)
                {
                    return;
                }
            }

            //Формируем строку вида "X:", где, X - буква интересующего диска
            string drive_name = letter + ":";

            //Отключаем стандартный вывод ошибок в мессаджбоксах
            //Это необходимо для того, если мы наткнемся на отсутствующий диск, имеющий
            //тем не менее букву (кардридер без вставленной карты, например)
            ErrorModes old_mode = SetErrorMode(ErrorModes.SEM_FAILCRITICALERRORS);

            //Открываем устройство с флагом FILE_FLAG_BACKUP_SEMANTICS
            IntPtr device_handle = CreateFileW(
                                    drive_name,
                                    EFileAccess.FILE_GENERIC_READ,
                                    EFileShare.Read | EFileShare.Delete | EFileShare.Write,
                                    IntPtr.Zero,
                                    ECreationDisposition.OpenExisting,
                                    EFileAttributes.BackupSemantics,
                                    IntPtr.Zero);

            //Возвращаем уровень ошибок на прежний
            SetErrorMode(old_mode);

            //Если какая-нибудь ошибка - выходим из функции
            if (device_handle.ToInt32() == INVALID_HANDLE_VALUE)
            return;
           

            IntPtr response = IntPtr.Zero;

            // Allocate response buffer for native call
            response = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DEV_BROADCAST_HANDLE)));


            //Готовимся настроить уведомления
            DEV_BROADCAST_HANDLE NotificationFilter = new DEV_BROADCAST_HANDLE();                 

            //уведомление
            NotificationFilter.dbch_size = Marshal.SizeOf(typeof(DEV_BROADCAST_HANDLE));
            NotificationFilter.dbch_devicetype = DBT_DEVTYP_HANDLE;
            NotificationFilter.dbch_handle = device_handle;

            Marshal.StructureToPtr(NotificationFilter, response, false);


            //Регистрируем оповещения о событиях с хендлом устройства
            //Последний флаг (DEVICE_NOTIFY_WINDOW_HANDLE) говорит о том, что
            //сообщения будут приходить нам в оконную процедуру
            IntPtr token = RegisterDeviceNotification(mon_hwnd, response, 
                                                    DEVICE_NOTIFY_WINDOW_HANDLE);

           


            if (token.ToInt32() == INVALID_HANDLE_VALUE)
            {
                //Если ошибка - закрываем хендл и выходим из функции
                CloseHandle(device_handle);
                return;
            }

            NotificationFilter.dbch_hdevnotify = token;

            //Запоминаем созданный хендл вместе с нотификацией и буквой диска
            exist_notifications.Add(device_handle, Tuple.Create(token, letter));

            //Если задан пользовательский коллбек, дернем его
            //и сообщим о том, что добавлен новый девайс
            if (on_device_adds.GetInvocationList().GetLength(0) != 0)
            {
                on_device_adds(letter);
            }

            Marshal.FreeHGlobal(response);
        }


        //убрать из контроля
        private void unmount_device(char letter, bool call_unsafe_callback)
        {

            //Проверяем, не подконтрольно ли нам уже это устройство
            foreach (KeyValuePair<IntPtr, Tuple<IntPtr, char>> note in exist_notifications)
            {
                if (note.Value.Item2 == letter)
                {
                    //Снимаем регистрацию событий
                    UnregisterDeviceNotification(note.Value.Item1);
                    
                    //Закрываем хендл устройства            
                    CloseHandle(note.Key);
                    
                    //Удаляем информацию об устройстве
                    exist_notifications.Remove(note.Key);

                    //Если надо - дергаем коллбек о небезопасном извлечении
                    if (call_unsafe_callback && on_device_removes.GetInvocationList().GetLength(0) != 0)
                    {
                        on_device_removes(letter);
                    }

                    break;
                }
            }


        }

       

        //Эта функция возвращает информацию об устройстве по букве диска
        public static device_info get_device_info(char letter)
        {
          
            StringBuilder volume_access_path = new StringBuilder("\\\\.\\X:");                       
             //Формируем строку вида \\.\X: для устройства
            volume_access_path[4] = letter;

            //Открываем его
            IntPtr vol = CreateFileW(volume_access_path.ToString(), 0,
                EFileShare.Read | EFileShare.Write,
                IntPtr.Zero, ECreationDisposition.OpenExisting, 0, IntPtr.Zero);

            //Если ошибка - бросим исключение
            if (vol.ToInt32() == INVALID_HANDLE_VALUE)
                throw new Exception("Cannot open device");

            //Теперь надо получить номер устройства
            STORAGE_DEVICE_NUMBER sdn = new STORAGE_DEVICE_NUMBER();





            int bytes_ret = 0;
            int DeviceNumber = -1;

            //Это делается таким IOCTL-запросом к устройству
            if (DeviceIoControl(vol.ToInt32(),
                IOCTL_STORAGE_GET_DEVICE_NUMBER,
                null, 0, ref sdn, Marshal.SizeOf(typeof(STORAGE_DEVICE_NUMBER)),
                ref bytes_ret, IntPtr.Zero) != 0)
            {
                DeviceNumber = sdn.DeviceNumber;
            }


            //Хендл нам больше не нужен
            CloseHandle(vol);


            //Если номер не получен - ошибка
            if (DeviceNumber == -1)
                throw new Exception("Cannot get device number");



            //Еще две вспомогательные строки вида X: и X:\
            StringBuilder devname = new StringBuilder("?:");
            StringBuilder devpath = new StringBuilder("?:\\");

            devname[0] = letter;
            devpath[0] = letter;

            char[] dos_name = new char[MAX_PATH + 1];

            string s = dos_name.ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append(s);

            IntPtr response = IntPtr.Zero;

            // Allocate response buffer for native call
            response = Marshal.AllocHGlobal(MAX_PATH);

            //Этот момент уже описан выше - используется для определения
            //флешек и флопиков
            QueryDosDeviceW(devname.ToString(), response, MAX_PATH);

            if (response == IntPtr.Zero)
                throw new Exception("Cannot get device info");

            string str = Marshal.PtrToStringAnsi(response, MAX_PATH);

            bool floppy = str.Contains("\\Floppy");


            Marshal.FreeHGlobal(response);

            //Определяем тип устройства
            DriveType drive_type = GetDriveType(devpath.ToString());


            Guid guid;
            //Теперь выясним класс устройства, с которым имеем дело

            switch(drive_type)
            {
            case DriveType.Removable:
                if(floppy)
                    guid = GUID_DEVINTERFACE.GUID_DEVINTERFACE_FLOPPY; //флоппи
                else
                    guid = GUID_DEVINTERFACE.GUID_DEVINTERFACE_DISK; //какой-то диск
                break;

            case DriveType.Fixed:
                    guid = GUID_DEVINTERFACE.GUID_DEVINTERFACE_DISK; //какой-то диск
                break;

            case DriveType.CDROM:
                    guid = GUID_DEVINTERFACE.GUID_DEVINTERFACE_CDROM; //CD-ROM
                break;

            default:
                throw new Exception("Unknown device"); //Неизвестный тип
            }


            //Получаем хендл к набору различных сведений о классе устройств info.dev_class на локальном компьютере,
            //выше эта функция уже была упомянута
            IntPtr dev_info = SetupDiGetClassDevs(ref guid, IntPtr.Zero,
                                                        IntPtr.Zero, DiGetClassFlags.DIGCF_PRESENT |
                                                        DiGetClassFlags.DIGCF_DEVICEINTERFACE);


            //Если что-то не так, кинем исключение
            if (dev_info.ToInt32() == INVALID_HANDLE_VALUE)
                throw new Exception("Cannot get device class");


            uint index = 0;
            bool ret = false;           

            SP_DEVICE_INTERFACE_DETAIL_DATA pspdidd
               = new SP_DEVICE_INTERFACE_DETAIL_DATA();

            SP_DEVICE_INTERFACE_DATA spdid = new SP_DEVICE_INTERFACE_DATA();
            SP_DEVINFO_DATA spdd = new SP_DEVINFO_DATA();
            uint size;

            spdid.cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA));
            bool found = false;


            //устройство
            IntPtr drive = new IntPtr();

            while (true)
            {
                //Перечисляем все устройства заданного класса
                ret = SetupDiEnumDeviceInterfaces(dev_info, IntPtr.Zero,
                                                    ref guid, index, ref spdid);
                
                
                if (!ret)
                break;

                //Получаем размер данных об устройстве
                size = 0;


                bool res = SetupDiGetDeviceInterfaceDetail(dev_info, ref spdid, IntPtr.Zero,
                                        0, out size, IntPtr.Zero);


                if (size != 0 && size <= 1024)
                {
                    
                    //pspdidd.cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DETAIL_DATA));

                    
                    if (IntPtr.Size == 8) // for 64 bit operating systems
                    {
                        pspdidd.cbSize = 8;
                    }
                    else
                    {
                        pspdidd.cbSize = 4 + Marshal.SystemDefaultCharSize; // for 32 bit systems
                    }                   


                    //CoreDll.ZeroMemory(spdd);

                    
                    spdd.cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));

                    //А теперь получаем информацию об устройстве
                    res = SetupDiGetDeviceInterfaceDetail(dev_info, ref spdid,
                                                    ref pspdidd, size, 
                                                    out size, ref spdd);



                    int word = Marshal.GetLastWin32Error();

                    if (res)
                    {
                        //Если все окей, открываем девайс по пути, который узнали
                        drive = CreateFileW(pspdidd.DevicePath, 0,
                            EFileShare.Read | EFileShare.Write,
                            IntPtr.Zero, ECreationDisposition.OpenExisting, 0, IntPtr.Zero);

                        if (drive.ToInt32() != INVALID_HANDLE_VALUE)
                        {
                            //Получаем номер устройства, и если он совпадает
                            //с определенным нами ранее,
                            //то нужное устройство мы нашли
                            int bytes_returned = 0;

                                //Это делается таким IOCTL-запросом к устройству
                                if (DeviceIoControl(drive.ToInt32(),
                                    IOCTL_STORAGE_GET_DEVICE_NUMBER,
                                    null, 0, ref sdn, Marshal.SizeOf(typeof(STORAGE_DEVICE_NUMBER)),
                                    ref bytes_returned, IntPtr.Zero) != 0)
                                {
                                    if (DeviceNumber == sdn.DeviceNumber)
                                    {
                                        //Если нашли, то выходим из цикла
                                        CloseHandle(drive); //Позволяет извлечь
                                        found = true;
                                        break;
                                    }
                                }

                                CloseHandle(drive); //Позволяет извлечь
                        }

                    }

                }

                index++;

            }

            SetupDiDestroyDeviceInfoList(dev_info);

            //А если не нашли устройство - то кинем эксепшен
            if (!found)
            throw new Exception("Cannot find device");


            //Находим родителя устройства
            //Например, USB-хаб для флешки
            uint dev_parent;


            if (CR_SUCCESS != CM_Get_Parent(out dev_parent, spdd.devInst, 0))
            {
                throw new Exception("Cannot get device parent");
            }




            //выделение памяти под значения
            IntPtr szDeviceInstanceID = Marshal.AllocHGlobal(MAX_PATH);
            IntPtr szNtDeviceName = Marshal.AllocHGlobal(MAX_PATH);


            //получение ID устройства
            int result = CM_Get_Device_ID(dev_parent, szDeviceInstanceID, MAX_PATH, 0);


            SP_DEVINFO_DATA data = new SP_DEVINFO_DATA();

            //структура информации о диске
            info_disk disk = new info_disk();

            if (result == CR_SUCCESS)
            {

                //TODO 
                //======================================================

                //работает Parent Device Instance ID!!!
                disk.Parent_Device_Instance_ID = Marshal.PtrToStringAuto(szDeviceInstanceID);

            }
            else
            {
                throw new Exception("Cannot get device parent");
            }


            //======================================================

                //работает Dos Device Name!!!
                if (response == IntPtr.Zero)
                    throw new Exception("Cannot get device info");

                uint returnSize = QueryDosDevice(devname.ToString(), 
                    szNtDeviceName, MAX_PATH);

                int error = Marshal.GetLastWin32Error();

                if (szNtDeviceName != IntPtr.Zero && returnSize != 0)
                {

                    disk.Dos_Device_Name = Marshal.PtrToStringAnsi(szNtDeviceName, (int)returnSize - 2);                    
                }

                //======================================================
                // ТИП ШИНЫ              
                /*
                //
                STORAGE_DEVICE_DESCRIPTOR sbt = new STORAGE_DEVICE_DESCRIPTOR();
                STORAGE_PROPERTY_QUERY spq = new STORAGE_PROPERTY_QUERY();

                spq.PropertyId = STORAGE_PROPERTY_ID.StorageDeviceProperty;
                spq.QueryType  = STORAGE_QUERY_TYPE.PropertyStandardQuery;
                spq.AdditionalParameters = '0';

                ///
                //открыть файл по пути 
                drive = CreateFileW(pspdidd.DevicePath, 0,
                            EFileShare.Read | EFileShare.Write,
                            IntPtr.Zero, ECreationDisposition.OpenExisting, 0, IntPtr.Zero);
                ///
                

                if (drive.ToInt32() != INVALID_HANDLE_VALUE)
                {

                    int bytes_returned = 0;

                    int sc = DeviceIoControl(drive.ToInt32(),
                                        IOCTL_STORAGE_QUERY_PROPERTY,
                                        ref spq, Marshal.SizeOf(typeof(STORAGE_PROPERTY_QUERY)),
                                        ref sbt, Marshal.SizeOf(typeof(STORAGE_DEVICE_DESCRIPTOR)),
                                        ref bytes_returned, IntPtr.Zero);


                    error = Marshal.GetLastWin32Error();

                    if (sc != 0)
                    {
                        CloseHandle(drive);
                        STORAGE_BUS_TYPE type = sbt.BusType;
                        disk.Bus_Type = typeBus(type);
                    }
                }
               
            */            

            //высвобождение памяти
            Marshal.FreeHGlobal(szDeviceInstanceID);
            Marshal.FreeHGlobal(szNtDeviceName);


            //===================================
            //возможно неправильно работает
            //===================================
            //SP_DEVINFO_DATA data = ByteArrayToStructure<SP_DEVINFO_DATA>
            //    (BitConverter.GetBytes(dev_parent).Reverse().ToArray())

            //Заполняем нашу структуру всякой интересной
            //информацией об устройстве
            device_info info = new device_info();
            info.dev_class  = guid;
            info.dev_inst   = data;
            info.info_disk  = disk;
            info.dev_number = (uint)(int)DeviceNumber;

            return info;
        }


        private static String typeBus(STORAGE_BUS_TYPE type) 
        {
            String[] arStorageBusTypeNames = new String[16]
            {
		        "Unknown",					  // BusTypeUnknown = 0
		        "SCSI",						  // BusTypeScsi = 1
		        "ATAPI",				      // BusTypeAtapi = 2
		        "ATA",						  // BusTypeAta = 3
		        "IEEE-1394",			      // BusType1394 = 4
		        "SSA",						  // BusTypeSsa = 5
		        "Fibre Channel",			  // BusTypeFibre = 6
		        "USB",						  // BusTypeUsb = 7
		        "RAID",						  // BusTypeRAID = 8
		        "iSCSI",					  // BusTypeiScsi = 9
		        "Serial Attached SCSI (SAS)", // BusTypeSas = 10
		        "SATA",						  // BusTypeSata = 11
		        "SD",						  // BusTypeSd = 12
		        "MMC",						  // BusTypeMmc = 13
		        "Virtual",					  // BusTypeVirtual = 14
		        "FileBackedVirtual"			  // BusTypeFileBackedVirtual = 15
	       };

            if ((int)type > 0 && (int)type < 16)
                return arStorageBusTypeNames[(int)type];
            else return "Unknown";


        }


        //из байтового массива в структуру
        private static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(),
                typeof(T));
            handle.Free();
            return stuff;
        }


        /*
         * Добавление
         *  
         * Удаление делегатов
        */
        //создание делегатов
        public delegate void on_device_added_(char c);
        public delegate void on_device_removed_(char c);
        public delegate bool on_device_safe_removed_(char c);
        public delegate void on_device_remove_failed_(char c);


        //тип делегата
        private on_device_added_ on_device_adds;
        private on_device_removed_ on_device_removes;
        private on_device_safe_removed_ on_device_safe_removes;
        private on_device_remove_failed_ on_device_remove_failes;



        //Добавляет функцию, вызываемую при добавлении нового USB flash-диска
        public void on_device_add(on_device_added_ Method)
        {
            on_device_adds += Method;
        }

        ///Добавляет функцию, вызываемую при небезопасном извлечении USB flash-диска
        public void on_device_remove(on_device_removed_ Method)
        {
            on_device_removes += Method;
        }

        //Добавляет функцию, вызываемую при безопасном извлечении USB flash-диска
        public void on_device_safe_remove(on_device_safe_removed_ Method)
        {
            on_device_safe_removes += Method;
        }

        //Добавляет функцию, вызываемую при неудачном безопасном извлечении USB flash-диска
        public void on_device_remove_fail(on_device_remove_failed_ Method)
        {
            on_device_remove_failes += Method;
        }




        //Стартует отслеживание USB
        public void start()
        {
            started = true;
        }


        //Останавливает отслеживание USB
        public void stop()
        {
            started = false;
        }

        //Запущено ли отслеживание USB
        public bool is_started()
        {
            return started;
        }

        //Взять под контроль существующие USB-флешки
        //Если устройство уже было замонтировано, ничего не произойдет
        //Для каждого замонтированного устройства будет вызван коллбек on_device_add
        public void mount_existing_devices()
        {
            //Получаем список всех устройств
            List<char> devices = get_flash_disks(monitor_hard_drives);
            //Освобождаем найденные устройства. Если устройство не под контролем, то
            //unmount_device просто ничего не сделает
            foreach (char device in devices)
            {
                mount_device(device);
            }
        }

        //Освободить все флешки, которые ранее были взяты под контроль
        //Коллбеки не вызывает
        public void unmount_all_devices()
        {
            //Получаем список всех устройств
            List<char> devices = get_flash_disks(monitor_hard_drives);
            //Освобождаем найденные устройства. Если устройство не под контролем, то
            //unmount_device просто ничего не сделает
            foreach(char device in devices)
            {
                unmount_device(device, false);
            }
        }

        //Безопасно извлечь какое-либо устройство
        //Коллбеков не вызывает
        public void safe_eject(char letter)
        {
            //Хендл экземпляра девайса для локальной машины 
            SP_DEVINFO_DATA dev = get_device_info(letter).dev_inst;

            //Проверим, не подконтролен ли нам этот девайс, и если это так, освободим его
            unmount_device(letter, false);

            int Empty = 0;

            //Вызываем функцию безопасного извлечения. 2-5 параметры не передаем, чтобы
            //проводник смог сообщить пользователю о том, что смог/не смог извлечь устройство
            if (CR_SUCCESS != CM_Request_Device_EjectW(dev, ref Empty, null, 0, 0))
            {
                throw new Exception("Cannot safe-eject device");
            }
        }

        



        //Установить опцию - отключать ли безопасно USB-устройство даже в том случае,
        //если после запроса на отключение от Windows прошел таймаут ожидания ответа
        //от приложения
        //По умолчанию включено
        public void set_safe_remove_on_timeout(bool remove)
        {
            safe_remove_on_timeout = remove;
        }

        //Включена ли опция безопасного отключения после таймаута ожидания Windows
        public bool is_set_safe_remove_on_timeout_on()
        {
            return safe_remove_on_timeout;
        }



        /// Получить буквы всех USB flash-дисков, имеющихся в системе в данный момент времени
        /// Если include_usb_hard_drives == true, то в список попадут буквы внешних жестких дисков,
        /// в противном случае - только флешки
        /// <summary>
        /// Когда добавляется флешка, то у жестких дисков меняется политика способности к выводу
        /// </summary>
        /// <param name="include_usb_hard_drives"></param>
        /// <returns></returns>
        public static List<char> get_flash_disks(bool include_usb_hard_drives)
        {
            List<char> devices = new List<char>();


            List<string> ds = new List<string>();

            //Получаем список логических разделов
            uint disks = GetLogicalDrives();
            uint n;

            //Строка для формирования имен вида A:, B:, ...
            string drive_root;

            //Смотрим, какие логические разделы есть в системе   
            for (int i = 0; i < 26; i++)
            {
                //Если диск есть
                n = ((disks>>i)&0x00000001);
		        if( n == 1 ) 
                {
                    //Формируем строку с именем диска
                    drive_root = (char)('A' + i) + ":";
                    //Получаем тип устройства
                    DriveType type = GetDriveType(drive_root);

                    //Если это съемный девайс (флешка или флоппи)
                    if(type == DriveType.Removable)
                    {
                        //Получаем тип девайса - это, похоже, самый простой
                        //путь отличить флешку от флоппика
                        char[] buf = new char[MAX_PATH];

                        string s = new string(buf);
                        StringBuilder sb = new StringBuilder();
                        String rv = sb.ToString();
                        sb.Append(s);
                        rv = sb.ToString();

                        IntPtr response = IntPtr.Zero;

                        // Allocate response buffer for native call
                        response = Marshal.AllocHGlobal(MAX_PATH);

                        int sz = QueryDosDeviceW(drive_root, response, MAX_PATH);

                        int word = Marshal.GetLastWin32Error();

                        if (response != IntPtr.Zero)
                        {

                            string rep = Marshal.PtrToStringAnsi(response, MAX_PATH);
                            if (!rep.Contains("\\Floppy"))
                            {
                                devices.Add((char)('A' + i)); //то это флешка
                            }

                        }

                        Marshal.FreeHGlobal(response);

                    }
                    //===================================================

                    //Если это какой-то жесткий диск, и мы их тоже мониторим
                    else if (type == DriveType.Fixed && include_usb_hard_drives)
                    {

                        try
                        {
                            //Получаем информацию о девайсе
                            device_info info = get_device_info((char)('A' + i));
                            
                            //Получаем хендл к набору различных сведений о классе устройств info.dev_class на локальном компьютере
                            IntPtr dev_info = SetupDiGetClassDevs(ref info.dev_class, IntPtr.Zero,
                                                                    IntPtr.Zero, DiGetClassFlags.DIGCF_PRESENT | 
                                                                                 DiGetClassFlags.DIGCF_DEVICEINTERFACE);

                            //Если хендл получен
                            if (dev_info.ToInt32() != INVALID_HANDLE_VALUE)
                            {
                                SP_DEVINFO_DATA dev_data = new SP_DEVINFO_DATA();
                                dev_data.cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));

                                //Получаем информацию о жестком диске
                                if (SetupDiEnumDeviceInfo(dev_info, info.dev_number, ref dev_data))
                                {
                                    //IntPtr ptrBuffer = new IntPtr();
                                    //ptrBuffer = Marshal.AllocHGlobal(sizeof(int));

                                    byte[] properties = new byte[sizeof(int)];
                                    
                                    //Получаем информацию о свойстве SPDRP_REMOVAL_POLICY жесткого диска
                                    //Оно говорит о том, может ли устройство быть извлечено
                                    //Если может, добавим его в результирующий набор
                                    
                                    bool success = SetupDiGetDeviceRegistryProperty(dev_info, ref dev_data, SPDRP_REMOVAL_POLICY, IntPtr.Zero,
                                        properties, sizeof(int), IntPtr.Zero);

                                    if (!success)
                                    {
                                        int error = Marshal.GetLastWin32Error();
                                        if (error != 0)
                                        {
                                            // I may ignore this property or I may simply
                                            // go on, required size has been set in devClassPropRequiredSize
                                            // so next call should work as expected (or fail in a managed way).
                                        }
                                    }

                                    int props = BitConverter.ToInt32(properties, 0);

                                    //======================================================

                                    /// Почему-то жесткие диски меняют свою политику, когда добавляется 

                                    //======================================================
                                    //Жесткие диски не добавляются
                                    if (success && props != CM_REMOVAL_POLICY_EXPECT_NO_REMOVAL)
                                    {
                                        devices.Add((char)('A' + i));
                                    }                                    

                                }
                                //Освободим информационный хендл
                                SetupDiDestroyDeviceInfoList(dev_info);

                            }

                        }
                        catch (Exception ex)
                        {
                            //Ошибки тут игнорируем
                        }

                    }
                    //===================================================
                }
            }


            //Возвращаем набор
            return devices;

        }
        
   
        //Включить опцию отслеживания внешних жестких дисков
        //По умолчанию включена опция отслеживания только USB-флешек
        public void enable_usb_hard_drive_monitoring(bool enable)
        {
            monitor_hard_drives = enable;
        }
 
        //Включена ли опция отслеживания внешних жестких дисков
        public bool is_usb_hard_drive_monitoring_enabled()
        {
            return monitor_hard_drives;
        }

        //установить хендл окна
        public void setHwnd(IntPtr hd)
        {
            mon_hwnd = hd;
        }



    }

    public class CoreDll
    {
        [DllImport("MSVCRT.dll", EntryPoint = "memset", SetLastError = false)]
        private static extern void memset(IntPtr dest, int c, int size);

        public static bool ZeroMemory(object o)
        {
            try
            {
                GCHandle gc = GCHandle.Alloc(o, GCHandleType.Pinned);
                memset(gc.AddrOfPinnedObject(), 0, Marshal.SizeOf(o));
                gc.Free();
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (NotImplementedException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }
    }

    public class GUID_DEVINTERFACE
    {
        public static Guid BUS1394_CLASS_GUID = new Guid("6BDD1FC1-810F-11d0-BEC7-08002BE2092F");
        public static Guid GUID_61883_CLASS = new Guid("7EBEFBC0-3200-11d2-B4C2-00A0C9697D07");
        public static Guid GUID_DEVICE_APPLICATIONLAUNCH_BUTTON = new Guid("629758EE-986E-4D9E-8E47-DE27F8AB054D");
        public static Guid GUID_DEVICE_BATTERY = new Guid("72631E54-78A4-11D0-BCF7-00AA00B7B32A");
        public static Guid GUID_DEVICE_LID = new Guid("4AFA3D52-74A7-11d0-be5e-00A0C9062857");
        public static Guid GUID_DEVICE_MEMORY = new Guid("3FD0F03D-92E0-45FB-B75C-5ED8FFB01021");
        public static Guid GUID_DEVICE_MESSAGE_INDICATOR = new Guid("CD48A365-FA94-4CE2-A232-A1B764E5D8B4");
        public static Guid GUID_DEVICE_PROCESSOR = new Guid("97FADB10-4E33-40AE-359C-8BEF029DBDD0");
        public static Guid GUID_DEVICE_SYS_BUTTON = new Guid("4AFA3D53-74A7-11d0-be5e-00A0C9062857");
        public static Guid GUID_DEVICE_THERMAL_ZONE = new Guid("4AFA3D51-74A7-11d0-be5e-00A0C9062857");
        public static Guid GUID_BTHPORT_DEVICE_INTERFACE = new Guid("0850302A-B344-4fda-9BE9-90576B8D46F0");
        public static Guid GUID_DEVINTERFACE_BRIGHTNESS = new Guid("FDE5BBA4-B3F9-46FB-BDAA-0728CE3100B4");
        public static Guid GUID_DEVINTERFACE_DISPLAY_ADAPTER = new Guid("5B45201D-F2F2-4F3B-85BB-30FF1F953599");
        public static Guid GUID_DEVINTERFACE_I2C = new Guid("2564AA4F-DDDB-4495-B497-6AD4A84163D7");
        public static Guid GUID_DEVINTERFACE_IMAGE = new Guid("6BDD1FC6-810F-11D0-BEC7-08002BE2092F");
        public static Guid GUID_DEVINTERFACE_MONITOR = new Guid("E6F07B5F-EE97-4a90-B076-33F57BF4EAA7");
        public static Guid GUID_DEVINTERFACE_OPM = new Guid("BF4672DE-6B4E-4BE4-A325-68A91EA49C09");
        public static Guid GUID_DEVINTERFACE_VIDEO_OUTPUT_ARRIVAL = new Guid("1AD9E4F0-F88D-4360-BAB9-4C2D55E564CD");
        public static Guid GUID_DISPLAY_DEVICE_ARRIVAL = new Guid("1CA05180-A699-450A-9A0C-DE4FBE3DDD89");
        public static Guid GUID_DEVINTERFACE_HID = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");
        public static Guid GUID_DEVINTERFACE_KEYBOARD = new Guid("884b96c3-56ef-11d1-bc8c-00a0c91405dd");
        public static Guid GUID_DEVINTERFACE_MOUSE = new Guid("378DE44C-56EF-11D1-BC8C-00A0C91405DD");
        public static Guid GUID_DEVINTERFACE_MODEM = new Guid("2C7089AA-2E0E-11D1-B114-00C04FC2AAE4");
        public static Guid GUID_DEVINTERFACE_NET = new Guid("CAC88484-7515-4C03-82E6-71A87ABAC361");
        public static Guid GUID_DEVINTERFACE_SENSOR = new Guid(0XBA1BB692, 0X9B7A, 0X4833, 0X9A, 0X1E, 0X52, 0X5E, 0XD1, 0X34, 0XE7, 0XE2);
        public static Guid GUID_DEVINTERFACE_COMPORT = new Guid("86E0D1E0-8089-11D0-9CE4-08003E301F73");
        public static Guid GUID_DEVINTERFACE_PARALLEL = new Guid("97F76EF0-F883-11D0-AF1F-0000F800845C");
        public static Guid GUID_DEVINTERFACE_PARCLASS = new Guid("811FC6A5-F728-11D0-A537-0000F8753ED1");
        public static Guid GUID_DEVINTERFACE_SERENUM_BUS_ENUMERATOR = new Guid("4D36E978-E325-11CE-BFC1-08002BE10318");
        public static Guid GUID_DEVINTERFACE_CDCHANGER = new Guid("53F56312-B6BF-11D0-94F2-00A0C91EFB8B");
        public static Guid GUID_DEVINTERFACE_CDROM = new Guid("53F56308-B6BF-11D0-94F2-00A0C91EFB8B");
        public static Guid GUID_DEVINTERFACE_DISK = new Guid("53F56307-B6BF-11D0-94F2-00A0C91EFB8B");
        public static Guid GUID_DEVINTERFACE_FLOPPY = new Guid("53F56311-B6BF-11D0-94F2-00A0C91EFB8B");
        public static Guid GUID_DEVINTERFACE_MEDIUMCHANGER = new Guid("53F56310-B6BF-11D0-94F2-00A0C91EFB8B");
        public static Guid GUID_DEVINTERFACE_PARTITION = new Guid("53F5630A-B6BF-11D0-94F2-00A0C91EFB8B");
        public static Guid GUID_DEVINTERFACE_STORAGEPORT = new Guid("2ACCFE60-C130-11D2-B082-00A0C91EFB8B");
        public static Guid GUID_DEVINTERFACE_TAPE = new Guid("53F5630B-B6BF-11D0-94F2-00A0C91EFB8B");
        public static Guid GUID_DEVINTERFACE_VOLUME = new Guid("53F5630D-B6BF-11D0-94F2-00A0C91EFB8B");
        public static Guid GUID_DEVINTERFACE_WRITEONCEDISK = new Guid("53F5630C-B6BF-11D0-94F2-00A0C91EFB8B");
        public static Guid GUID_IO_VOLUME_DEVICE_INTERFACE = new Guid("53F5630D-B6BF-11D0-94F2-00A0C91EFB8B");
        public static Guid MOUNTDEV_MOUNTED_DEVICE_GUID = new Guid("53F5630D-B6BF-11D0-94F2-00A0C91EFB8B");
        public static Guid GUID_AVC_CLASS = new Guid("095780C3-48A1-4570-BD95-46707F78C2DC");
        public static Guid GUID_VIRTUAL_AVC_CLASS = new Guid("616EF4D0-23CE-446D-A568-C31EB01913D0");
        public static Guid KSCATEGORY_ACOUSTIC_ECHO_CANCEL = new Guid("BF963D80-C559-11D0-8A2B-00A0C9255AC1");
        public static Guid KSCATEGORY_AUDIO = new Guid("6994AD04-93EF-11D0-A3CC-00A0C9223196");
        public static Guid KSCATEGORY_AUDIO_DEVICE = new Guid("FBF6F530-07B9-11D2-A71E-0000F8004788");
        public static Guid KSCATEGORY_AUDIO_GFX = new Guid("9BAF9572-340C-11D3-ABDC-00A0C90AB16F");
        public static Guid KSCATEGORY_AUDIO_SPLITTER = new Guid("9EA331FA-B91B-45F8-9285-BD2BC77AFCDE");
        public static Guid KSCATEGORY_BDA_IP_SINK = new Guid("71985F4A-1CA1-11d3-9CC8-00C04F7971E0");
        public static Guid KSCATEGORY_BDA_NETWORK_EPG = new Guid("71985F49-1CA1-11d3-9CC8-00C04F7971E0");
        public static Guid KSCATEGORY_BDA_NETWORK_PROVIDER = new Guid("71985F4B-1CA1-11d3-9CC8-00C04F7971E0");
        public static Guid KSCATEGORY_BDA_NETWORK_TUNER = new Guid("71985F48-1CA1-11d3-9CC8-00C04F7971E0");
        public static Guid KSCATEGORY_BDA_RECEIVER_COMPONENT = new Guid("FD0A5AF4-B41D-11d2-9C95-00C04F7971E0");
        public static Guid KSCATEGORY_BDA_TRANSPORT_INFORMATION = new Guid("A2E3074F-6C3D-11d3-B653-00C04F79498E");
        public static Guid KSCATEGORY_BRIDGE = new Guid("085AFF00-62CE-11CF-A5D6-28DB04C10000");
        public static Guid KSCATEGORY_CAPTURE = new Guid("65E8773D-8F56-11D0-A3B9-00A0C9223196");
        public static Guid KSCATEGORY_CLOCK = new Guid("53172480-4791-11D0-A5D6-28DB04C10000");
        public static Guid KSCATEGORY_COMMUNICATIONSTRANSFORM = new Guid("CF1DDA2C-9743-11D0-A3EE-00A0C9223196");
        public static Guid KSCATEGORY_CROSSBAR = new Guid("A799A801-A46D-11D0-A18C-00A02401DCD4");
        public static Guid KSCATEGORY_DATACOMPRESSOR = new Guid("1E84C900-7E70-11D0-A5D6-28DB04C10000");
        public static Guid KSCATEGORY_DATADECOMPRESSOR = new Guid("2721AE20-7E70-11D0-A5D6-28DB04C10000");
        public static Guid KSCATEGORY_DATATRANSFORM = new Guid("2EB07EA0-7E70-11D0-A5D6-28DB04C10000");
        public static Guid KSCATEGORY_DRM_DESCRAMBLE = new Guid("FFBB6E3F-CCFE-4D84-90D9-421418B03A8E");
        public static Guid KSCATEGORY_ENCODER = new Guid("19689BF6-C384-48fd-AD51-90E58C79F70B");
        public static Guid KSCATEGORY_ESCALANTE_PLATFORM_DRIVER = new Guid("74F3AEA8-9768-11D1-8E07-00A0C95EC22E");
        public static Guid KSCATEGORY_FILESYSTEM = new Guid("760FED5E-9357-11D0-A3CC-00A0C9223196");
        public static Guid KSCATEGORY_INTERFACETRANSFORM = new Guid("CF1DDA2D-9743-11D0-A3EE-00A0C9223196");
        public static Guid KSCATEGORY_MEDIUMTRANSFORM = new Guid("CF1DDA2E-9743-11D0-A3EE-00A0C9223196");
        public static Guid KSCATEGORY_MICROPHONE_ARRAY_PROCESSOR = new Guid("830A44F2-A32D-476B-BE97-42845673B35A");
        public static Guid KSCATEGORY_MIXER = new Guid("AD809C00-7B88-11D0-A5D6-28DB04C10000");
        public static Guid KSCATEGORY_MULTIPLEXER = new Guid("7A5DE1D3-01A1-452c-B481-4FA2B96271E8");
        public static Guid KSCATEGORY_NETWORK = new Guid("67C9CC3C-69C4-11D2-8759-00A0C9223196");
        public static Guid KSCATEGORY_PREFERRED_MIDIOUT_DEVICE = new Guid("D6C50674-72C1-11D2-9755-0000F8004788");
        public static Guid KSCATEGORY_PREFERRED_WAVEIN_DEVICE = new Guid("D6C50671-72C1-11D2-9755-0000F8004788");
        public static Guid KSCATEGORY_PREFERRED_WAVEOUT_DEVICE = new Guid("D6C5066E-72C1-11D2-9755-0000F8004788");
        public static Guid KSCATEGORY_PROXY = new Guid("97EBAACA-95BD-11D0-A3EA-00A0C9223196");
        public static Guid KSCATEGORY_QUALITY = new Guid("97EBAACB-95BD-11D0-A3EA-00A0C9223196");
        public static Guid KSCATEGORY_REALTIME = new Guid("EB115FFC-10C8-4964-831D-6DCB02E6F23F");
        public static Guid KSCATEGORY_RENDER = new Guid("65E8773E-8F56-11D0-A3B9-00A0C9223196");
        public static Guid KSCATEGORY_SPLITTER = new Guid("0A4252A0-7E70-11D0-A5D6-28DB04C10000");
        public static Guid KSCATEGORY_SYNTHESIZER = new Guid("DFF220F3-F70F-11D0-B917-00A0C9223196");
        public static Guid KSCATEGORY_SYSAUDIO = new Guid("A7C7A5B1-5AF3-11D1-9CED-00A024BF0407");
        public static Guid KSCATEGORY_TEXT = new Guid("6994AD06-93EF-11D0-A3CC-00A0C9223196");
        public static Guid KSCATEGORY_TOPOLOGY = new Guid("DDA54A40-1E4C-11D1-A050-405705C10000");
        public static Guid KSCATEGORY_TVAUDIO = new Guid("A799A802-A46D-11D0-A18C-00A02401DCD4");
        public static Guid KSCATEGORY_TVTUNER = new Guid("A799A800-A46D-11D0-A18C-00A02401DCD4");
        public static Guid KSCATEGORY_VBICODEC = new Guid("07DAD660-22F1-11D1-A9F4-00C04FBBDE8F");
        public static Guid KSCATEGORY_VIDEO = new Guid("6994AD05-93EF-11D0-A3CC-00A0C9223196");
        public static Guid KSCATEGORY_VIRTUAL = new Guid("3503EAC4-1F26-11D1-8AB0-00A0C9223196");
        public static Guid KSCATEGORY_VPMUX = new Guid("A799A803-A46D-11D0-A18C-00A02401DCD4");
        public static Guid KSCATEGORY_WDMAUD = new Guid("3E227E76-690D-11D2-8161-0000F8775BF1");
        public static Guid KSMFT_CATEGORY_AUDIO_DECODER = new Guid("9ea73fb4-ef7a-4559-8d5d-719d8f0426c7");
        public static Guid KSMFT_CATEGORY_AUDIO_EFFECT = new Guid("11064c48-3648-4ed0-932e-05ce8ac811b7");
        public static Guid KSMFT_CATEGORY_AUDIO_ENCODER = new Guid("91c64bd0-f91e-4d8c-9276-db248279d975");
        public static Guid KSMFT_CATEGORY_DEMULTIPLEXER = new Guid("a8700a7a-939b-44c5-99d7-76226b23b3f1");
        public static Guid KSMFT_CATEGORY_MULTIPLEXER = new Guid("059c561e-05ae-4b61-b69d-55b61ee54a7b");
        public static Guid KSMFT_CATEGORY_OTHER = new Guid("90175d57-b7ea-4901-aeb3-933a8747756f");
        public static Guid KSMFT_CATEGORY_VIDEO_DECODER = new Guid("d6c02d4b-6833-45b4-971a-05a4b04bab91");
        public static Guid KSMFT_CATEGORY_VIDEO_EFFECT = new Guid("12e17c21-532c-4a6e-8a1c-40825a736397");
        public static Guid KSMFT_CATEGORY_VIDEO_ENCODER = new Guid("f79eac7d-e545-4387-bdee-d647d7bde42a");
        public static Guid KSMFT_CATEGORY_VIDEO_PROCESSOR = new Guid("302ea3fc-aa5f-47f9-9f7a-c2188bb16302");
        public static Guid GUID_DEVINTERFACE_USB_DEVICE = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");
        public static Guid GUID_DEVINTERFACE_USB_HOST_CONTROLLER = new Guid("3ABF6F2D-71C4-462A-8A92-1E6861E6AF27");
        public static Guid GUID_DEVINTERFACE_USB_HUB = new Guid("F18A0E88-C30C-11D0-8815-00A0C906BED8");
        public static Guid GUID_DEVINTERFACE_WPD = new Guid("6AC27878-A6FA-4155-BA85-F98F491D4F33");
        public static Guid GUID_DEVINTERFACE_WPD_PRIVATE = new Guid("BA0C718F-4DED-49B7-BDD3-FABE28661211");
        public static Guid GUID_DEVINTERFACE_SIDESHOW = new Guid("152E5811-FEB9-4B00-90F4-D32947AE1681");
    }












}
