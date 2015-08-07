using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;


namespace USBWinAPI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<char> disks;

        /// <summary>
        /// главное окно
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// загрузка окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            WindowInteropHelper wh = new WindowInteropHelper(this);
            //WindowInteropHelper - способствует взаимодействию Win32 и WPF
            HwndSource source = HwndSource.FromHwnd(wh.Handle);


            //установить хендл окна
            USB.Instance.setHwnd(wh.Handle);
            //добавляется хук на WndProc
            source.AddHook(new HwndSourceHook(USB.Instance.WndProc));

            USB.Instance.on_device_add(device_added);
            USB.Instance.on_device_remove(device_removed);
            USB.Instance.on_device_safe_remove(device_safe_removed);
            USB.Instance.on_device_remove_fail(device_remove_failed);

            //USB.Instance.enable_usb_hard_drive_monitoring(false);
            disks = USB.get_flash_disks(false);

            foreach (char disk in disks)
            {
                cbName.Items.Add(disk+":");
            }

            if (cbName.Items.Count > 0)
            {
                cbName.SelectedIndex = 0;
            }
           
        }


        /// <summary>
        /// Будет вызвано при добавлении нового диска в систему
        /// </summary>
        /// <param name="letter"></param>
        void device_added(char letter)
        {
            txt.Text =  txt.Text + "Added USB disk: " + letter + "\n";
            if(!cbName.Items.Contains(letter + ":")) cbName.Items.Add(letter + ":");
            if (cbName.Items.Count > 0)
            {
                cbName.SelectedIndex = 0;
            }
        }


        /// <summary>
        /// Будет вызвано при небезопасном извлечении какого-либо диска
        /// </summary>
        /// <param name="letter"></param>
        void device_removed(char letter)
        {
            txt.Text =  txt.Text + "UNSAFE-removed USB disk: " + letter + "\n";
            cbName.Items.Remove(letter + ":");
            if (cbName.Items.Count > 0)
            {
                cbName.SelectedIndex = 0;
            }
        }


        /// <summary>
        /// Будет вызвано при безопасном извлечении какого-либо диска
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        bool device_safe_removed(char letter)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Разрешить извлечь диск", "Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)            
            {
                txt.Text =  txt.Text + "Safe-removed USB disk: " + letter + "\n";
                cbName.Items.Remove(letter + ":");
                if (cbName.Items.Count > 0)
                {
                    cbName.SelectedIndex = 0;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Будет вызвано при ошибке безопасного извлечении какого-либо диска
        /// (таймаут или запрет извлечения)
        /// </summary>
        /// <param name="letter"></param>
        void device_remove_failed(char letter)
        {
            txt.Text =  txt.Text +  "Failed to eject device: " + letter + "\n";
        }


        /// <summary>
        /// обработчик кнопки 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            cbName.Items.Clear();

            disks = USB.get_flash_disks(false);

            foreach (char disk in disks)
            {
                cbName.Items.Add(disk + ":");
            }

            if (cbName.Items.Count > 0)
            {
                cbName.SelectedIndex = 0;
            }

            //Определяем, какие флешки и usb-диски уже вставлены
            //и берем их под контроль
            USB.Instance.mount_existing_devices();
            //запускается мониторинг
            USB.Instance.start();

            txt.Text = "Start\n";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopMon_Click(object sender, RoutedEventArgs e)
        {
            //останавливается мониторинг
            USB.Instance.stop();
            //Флешки выводятся из контроля
            USB.Instance.unmount_all_devices();            
            txt.Text = "Stop\n";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MonHD_Click(object sender, RoutedEventArgs e)
        {          
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowInfo_Click(object sender, RoutedEventArgs e)
        {

            infoText.Text = "";

            if (cbName.Items.Count == 0)
            {
                return;
            }

            string str = cbName.Text;

            //Получаем информацию о девайсе
            device_info info = USB.get_device_info(str.ToArray()[0]);

            infoText.Text = str + " Type of device: ";
            if (info.dev_class == GUID_DEVINTERFACE.GUID_DEVINTERFACE_DISK)
            {
                infoText.Text = infoText.Text + "Устройство-накопитель\n";
            }
            infoText.Text = infoText.Text + "\n";
            //параметры диска
            infoText.Text = infoText.Text + "Type of device: ";
            string dev_class = info.dev_class.ToString();
            infoText.Text = infoText.Text + dev_class + "\n";
            infoText.Text = infoText.Text + "\n";
            //=================================================
            infoText.Text = infoText.Text + "devInst of parent: ";
            if (info.info_disk.Parent_Device_Instance_ID != null)
            {
                string dev_inst = info.info_disk.Parent_Device_Instance_ID.ToString();
                infoText.Text = infoText.Text + dev_inst + "\n";
            }
            infoText.Text = infoText.Text + "\n";
            //=================================================
            infoText.Text = infoText.Text + "Dos Device Name: ";
            if (info.info_disk.Dos_Device_Name != null)
            {
                string dos_name = info.info_disk.Dos_Device_Name.ToString();
                infoText.Text = infoText.Text + dos_name + "\n";
            }
            infoText.Text = infoText.Text + "\n";
            //=================================================
            infoText.Text = infoText.Text + "Bus Type: ";
            if (info.info_disk.Bus_Type != null)
            {
                string bus_type = info.info_disk.Bus_Type.ToString();
                infoText.Text = infoText.Text + bus_type + "\n";
            }
            infoText.Text = infoText.Text + "\n";
            //=====================================

            /*
            //=================================================
            infoText.Text = infoText.Text + "Class of parent: ";
            string classGuid = info.dev_inst.classGuid.ToString();
            infoText.Text = infoText.Text + classGuid + "\n";
            //=================================================
            infoText.Text = infoText.Text + "devInst of parent: ";
            string devInst = info.dev_inst.devInst.ToString();
            infoText.Text = infoText.Text + devInst + "\n";
            //=================================================
            infoText.Text = infoText.Text + "Device number: ";
            string dev_number = info.dev_number.ToString();
            infoText.Text = infoText.Text + dev_number + "\n";
            */
        }


        
    }
}
