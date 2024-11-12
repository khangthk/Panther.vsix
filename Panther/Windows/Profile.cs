using System.Collections.Generic;
using System.Linq;

namespace Panther.Windows
{
    internal static class Profile
    {
        public enum Type
        {
            BasicExe_Configurations,
            AllExe_Configurations,
            AllExe_Configurations_Other,
        }

        public static class Files
        {
            public static readonly List<string> BasicExe_Configurations = Configurations.Concat(BasicExe).Concat(AllDll).ToList();
            public static readonly List<string> AllExe_Configurations = Configurations.Concat(AllExe).Concat(AllDll).ToList();
            public static readonly List<string> AllExe_Configurations_Other = Configurations.Concat(Themes).Concat(SkullsDB).Concat(AllExe).Concat(AllDll).ToList();
        }

        public static string GetProfileName(Type type)
        {
            switch (type)
            {
                case Type.BasicExe_Configurations:
                    return "Profile 1 - Basic Exe + Configurations";
                case Type.AllExe_Configurations:
                    return "Profile 2 - All Exe + Configurations";
                case Type.AllExe_Configurations_Other:
                    return "Profile 3 - All Exe + Configurations + Other";
                default:
                    return string.Empty;
            }
        }

        private static readonly List<string> Configurations = new List<string>
        {
            "Configurations"
        };

        private static readonly List<string> Themes = new List<string>
        {
            "Documents",
            "Documents_Beijing Unicorn",
            "Documents_CybeRay",
            "Documents_CybeRay (TaiChi)",
            "Documents_Neusoft",
            "Documents_No Information",
            "Documents_Shinva",
            "Documents_SupeRay",
            "Documents_SupeRay (TaiChi)"
        };

        private static readonly List<string> SkullsDB = new List<string>
        {
            "Skulls-DB"
        };

        private static readonly List<string> BasicExe = new List<string>
        {
            "ConfigTool.exe",
            "MachineDataEntry.exe",
            "PantherTPS.exe"
        };

        private static readonly List<string> AllExe = new List<string>
        {
            "AMSManager.exe",
            "ConfigTool.exe",
            "DICOMCommunicator.exe",
            "DICOMServer.exe",
            "ImportTool.exe",
            "LicenseManager.exe",
            "LicenseServer.exe",
            "MachineDataEntry.exe",
            "PantherTPS.exe",
            "SecurityManager.exe",
            "WTCP.exe"
        };

        private static readonly List<string> AllDll = new List<string>
        {
            "ACE.dll",
            "AutoContour.dll",
            "AutoSeg.dll",
            "AutoSegBase.dll",
            "AutoSegResource.dll",
            "Base.dll",
            "BCGCBPRO3130u142.dll",
            "BCGPStyle2007Aqua3130.dll",
            "BCGPStyle2007Luna3130.dll",
            "BCGPStyle2007Obsidian3130.dll",
            "BCGPStyle2007Silver3130.dll",
            "BCGPStyle2010Black3130.dll",
            "BCGPStyle2010Blue3130.dll",
            "BCGPStyle2010White3130.dll",
            "BCGPStyleCarbon3130.dll",
            "BCGPStyleScenic3130.dll",
            "Beamlet_GPU_Calculation.dll",
            "CCCS Calculation.dll",
            "CCCS_GPU_Calculation.dll",
            "CoreCommon.dll",
            "cudart64_110.dll",
            "DeformableRegistration.dll",
            "DeformationRegistration.dll",
            "DICOMCommunicatorResources.dll",
            "DICOMImpExp.dll",
            "DICOMNew.dll",
            "Extractor2DImageFilter.dll",
            "GK Optimization.dll",
            "glew32.dll",
            "haspvlib_2146910.dll",
            "ImageBroker.dll",
            "IMRT Optimization.dll",
            "ITKCommon-5.1.dll",
            "ITKLabelMap-5.1.dll",
            "ITKOptimizers-5.1.dll",
            "ITKSmoothing-5.1.dll",
            "ITKStatistics-5.1.dll",
            "ITKTransform-5.1.dll",
            "Kokyu.dll",
            "LeopardDoseEngineLib.dll",
            "mimalloc-redirect.dll",
            "mimalloc.dll",
            "nvml.dll",
            "PantherCommon.dll",
            "Probdom.dll",
            "Resources.dll",
            "RTImpExpWizardLib.dll",
            "SecurityLib.dll",
            "SecurityR.dll",
            "StandardUI.dll",
            "TAO.dll",
            "TAO_AnyTypecode.dll",
            "TAO_PortableServer.dll",
            "tiff.dll",
            "tiffxx.dll",
            "UCDMC.dll",
            "UltChrtPro98.dll",
            "vtkCommonComputationalGeometry-7.0.dll",
            "vtkCommonCore-7.0.dll",
            "vtkCommonDataModel-7.0.dll",
            "vtkCommonExecutionModel-7.0.dll",
            "vtkCommonMath-7.0.dll",
            "vtkCommonMisc-7.0.dll",
            "vtkCommonSystem-7.0.dll",
            "vtkCommonTransforms-7.0.dll",
            "vtkFiltersCore-7.0.dll",
            "vtkFiltersExtraction-7.0.dll",
            "vtkFiltersGeneral-7.0.dll",
            "vtkFiltersGeometry-7.0.dll",
            "vtkFiltersModeling-7.0.dll",
            "vtkFiltersSources-7.0.dll",
            "vtksys-7.0.dll",
            "WTCP_DLL.dll",
            "zlib1.dll"
        };
    }
}
