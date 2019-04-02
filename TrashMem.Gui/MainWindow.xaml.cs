using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TrashMemCore;
using TrashMemGui.Gui.Objects;
using TrashMemGui.Objects;

namespace TrashMemGui.Gui
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TrashMem TrashMem { get; set; }
        public string DllPath { get; private set; }

        public MainWindow()
        {
            DllPath = AppDomain.CurrentDomain.BaseDirectory + "\\TrashMem.HookDll.dll";
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateProcesses();
        }

        private void TextboxAllocSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateProcesses();
        }

        private void ButtonAttach_Click(object sender, RoutedEventArgs e)
        {
            if (listboxProcesses != null)
            {
                if (TrashMem != null)
                {
                    TrashMem.Detach();
                    ResetTrashMemViews();
                }

                TrashMem = new TrashMem(((CProcess)listboxProcesses.SelectedItem).Process);

                labelTrashMemAttachedId.Content = $"{TrashMem.Process.Id} => 0x{TrashMem.Process.Id.ToString("X")}";
                labelTrashMemProcessHandle.Content = $"0x{TrashMem.ProcessHandle.ToString("X")}";
                labelTrashMemAllocs.Content = $"{TrashMem.MemoryAllocations.Count}";
                labelTrashMemCachedSizes.Content = $"{TrashMem.CachedSizeManager.SizeCache.Count}";
                labelTrashMemThreads.Content = $"{TrashMem.CachedSizeManager.SizeCache.Count}";
                labelKernel32Module.Content = $"0x{TrashMem.Kernel32ModuleHandle.ToString("X")}";
                labelLoadLibaryA.Content = $"0x{TrashMem.LoadLibraryAAddress.ToString("X")}";
            }
        }

        private void ButtonDetach_Click(object sender, RoutedEventArgs e)
        {
            if (TrashMem != null)
            {
                TrashMem.Detach();
            }

            ResetTrashMemViews();
        }

        private void ResetTrashMemViews()
        {
            labelTrashMemAttachedId.Content = "n/a";
            labelTrashMemProcessHandle.Content = "n/a";
            labelTrashMemAllocs.Content = "n/a";
            labelTrashMemCachedSizes.Content = "n/a";
            labelTrashMemThreads.Content = "n/a";
            labelKernel32Module.Content = "n/a";
            labelLoadLibaryA.Content = "n/a";

            labelAllocAddress.Content = "n/a";
            labelAllocSize.Content = "n/a";

            textboxByteView.Text = "";
            textboxDecodedByteView.Text = "";

            listboxAllocations.Items.Clear();

            labelInt16.Content = "n/a";
            labelInt32.Content = "n/a";
            labelInt64.Content = "n/a";
            labelFloat.Content = "n/a";
            labelDouble.Content = "n/a";
        }

        private void ListboxProcesses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listboxProcesses.SelectedItem != null)
            {
                labelProcessId.Content = $"{((CProcess)listboxProcesses.SelectedItem).Process.Id} => 0x{((CProcess)listboxProcesses.SelectedItem).Process.Id.ToString("X")}";
                labelProcessName.Content = $"{((CProcess)listboxProcesses.SelectedItem).Process.ProcessName}";
                labelProcessWindowTitle.Content = $"{((CProcess)listboxProcesses.SelectedItem).Process.MainWindowTitle}";
                try
                {
                    labelProcessBaseAddress.Content = $"0x{((CProcess)listboxProcesses.SelectedItem).Process.MainModule.BaseAddress.ToString("X")}";
                }
                catch (Win32Exception ex)
                {
                    labelProcessBaseAddress.Content = $"64bit not supported";
                }
            }
            else
            {
                labelProcessId.Content = "n/a";
                labelProcessName.Content = "n/a";
                labelProcessBaseAddress.Content = "n/a";
                labelProcessWindowTitle.Content = "n/a";
            }
        }

        private void UpdateProcesses()
        {
            listboxProcesses.Items.Clear();
            foreach (Process p in Process.GetProcesses().OrderBy(obj => obj.MainWindowTitle))
            {
                listboxProcesses.Items.Add(new CProcess(p));
            }
        }

        private void ButtonNewAlloc_Click(object sender, RoutedEventArgs e)
        {
            if (TrashMem != null)
            {
                if (int.TryParse(textboxAllocSize.Text, out int allocSize))
                {
                    TrashMem.AllocateMemory(allocSize);
                }

                UpdateAllocations();
            }
        }

        private void ButtonFreeAlloc_Click(object sender, RoutedEventArgs e)
        {
            if (TrashMem != null)
            {
                if (listboxAllocations.SelectedItem != null)
                {
                    TrashMem.FreeMemory((MemoryAllocation)listboxAllocations.SelectedItem);
                }

                UpdateAllocations();

                labelInt16.Content = "n/a";
                labelInt32.Content = "n/a";
                labelInt64.Content = "n/a";
                labelFloat.Content = "n/a";
                labelDouble.Content = "n/a";
            }
        }

        private void UpdateAllocations()
        {
            listboxAllocations.Items.Clear();
            foreach (MemoryAllocation memAlloc in TrashMem.MemoryAllocations)
            {
                listboxAllocations.Items.Add(memAlloc);
            }
            labelTrashMemAllocs.Content = $"{TrashMem.MemoryAllocations.Count}";
        }

        private void ListboxAllocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listboxAllocations.SelectedItem != null)
            {
                IntPtr address = ((MemoryAllocation)listboxAllocations.SelectedItem).Address;
                int size = ((MemoryAllocation)listboxAllocations.SelectedItem).Size;
                labelAllocAddress.Content = $"0x{address.ToString("X")}";
                labelAllocSize.Content = $"{size}";
                UpdateByteViews(address, size);
            }
            else
            {
                labelAllocAddress.Content = "n/a";
                labelAllocSize.Content = "n/a";
                textboxByteView.Text = "";
                textboxDecodedByteView.Text = "";
            }
        }

        private void UpdateByteViews(IntPtr address, int size)
        {
            byte[] bytes = TrashMem.ReadChars(address, size);
            textboxByteView.Text = GenerateByteView(bytes);
            textboxDecodedByteView.Text = GenerateEncodedByteView(bytes, Encoding.ASCII);

            labelInt16.Content = $"{BitConverter.ToInt16(bytes, 0)}";
            labelInt32.Content = $"{BitConverter.ToInt32(bytes, 0)}";
            labelInt64.Content = $"{BitConverter.ToInt64(bytes, 0)}";
            labelFloat.Content = $"{BitConverter.ToSingle(bytes, 0)}";
            labelDouble.Content = $"{BitConverter.ToDouble(bytes, 0)}";
        }

        private string GenerateByteView(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();

            int counter = 1;

            foreach (byte b in bytes)
            {
                string sByte = $"{b.ToString("X2")}";
                sb.Append(sByte);

                if (counter < 8)
                {
                    sb.Append($" ");
                }
                else
                {
                    counter = 0;
                    sb.AppendLine();
                }
                counter++;
            }

            return sb.ToString();
        }

        private string GenerateEncodedByteView(byte[] bytes, Encoding encoding)
        {
            char[] decodedBytes = encoding.GetChars(bytes);
            StringBuilder sb = new StringBuilder();

            int counter = 1;

            foreach (char c in decodedBytes)
            {
                sb.Append(c);

                if (counter == 8)
                {
                    counter = 0;
                    sb.AppendLine();
                }
                counter++;
            }

            return sb.ToString();
        }

        private void ButtonWriteData_Click(object sender, RoutedEventArgs e)
        {
            if (TrashMem != null && listboxAllocations.SelectedItem != null)
            {
                IntPtr address = ((MemoryAllocation)listboxAllocations.SelectedItem).Address;
                int size = ((MemoryAllocation)listboxAllocations.SelectedItem).Size;

                switch ((string)comboboxDataType.SelectedItem)
                {
                    case "Bytes":
                        byte[] bytes = Encoding.ASCII.GetBytes(textboxData.Text);
                        if (bytes.Length <= size)
                        {
                            TrashMem.WriteBytes(address, bytes);
                        }
                        break;

                    case "Char":
                        if (char.TryParse(textboxData.Text, out char charValue))
                        {
                            TrashMem.Write(address, charValue);
                        }
                        break;

                    case "Int16":
                        if (short.TryParse(textboxData.Text, out short shortValue))
                        {
                            TrashMem.Write(address, shortValue);
                        }
                        break;

                    case "Int32":
                        if (int.TryParse(textboxData.Text, out int intValue))
                        {
                            TrashMem.Write(address, intValue);
                        }
                        break;

                    case "Int64":
                        if (long.TryParse(textboxData.Text, out long longValue))
                        {
                            TrashMem.Write(address, longValue);
                        }
                        break;

                    case "Float":
                        if (float.TryParse(textboxData.Text, out float floatValue))
                        {
                            TrashMem.Write(address, floatValue);
                        }
                        break;

                    case "Double":
                        if (double.TryParse(textboxData.Text, out double doubleValue))
                        {
                            TrashMem.Write(address, doubleValue);
                        }
                        break;

                    case "ASCII String":
                        TrashMem.WriteString(address, textboxData.Text, Encoding.ASCII);
                        break;
                }

                UpdateByteViews(address, size);
            }
        }

        private void ButtonResetMemory_Click(object sender, RoutedEventArgs e)
        {
            if (TrashMem != null && listboxAllocations.SelectedItem != null)
            {
                IntPtr address = ((MemoryAllocation)listboxAllocations.SelectedItem).Address;
                int size = ((MemoryAllocation)listboxAllocations.SelectedItem).Size;

                TrashMem.WriteBytes(address, new byte[size]);
                UpdateByteViews(address, size);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (listboxProcesses.SelectedItem != null)
            {
                string folder = System.IO.Path.GetDirectoryName(
                    ((CProcess)listboxProcesses.SelectedItem).Process.MainModule.FileName);
                string filename = System.IO.Path.GetFileName(DllPath);

                if (!File.Exists($"{folder}\\{filename}"))
                {
                    File.Copy(DllPath, $"{folder}\\{filename}");
                }

                TrashMem.InjectDll(DllPath);

                UpdateAllocations();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                DllPath = openFileDialog.FileName;
            }

            labelDll.Content = System.IO.Path.GetFileName(DllPath);
        }
    }
}
