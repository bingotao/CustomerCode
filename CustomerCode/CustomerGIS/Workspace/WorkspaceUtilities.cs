using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingotao.Customer.GIS
{
    /// <summary>
    /// 工作空间类型枚举
    /// </summary>
    public enum enumWorkspaceType
    {
        SDE,
        SHAPE,
        MDB,
        GDB
    }

    /// <summary>
    /// 工作空间帮助类
    /// </summary>
    public class WorkspaceUtilities
    {
        /// <summary>
        /// 打开工作空间
        /// </summary>
        /// <param name="filePathOrConStr">ORACLE 直连传入连接字符串(INSTANCE=sde:oracle11g:127.0.0.1/ORCL.LOCALDOMAIN;USER=sde;PASSWORD=sde;VERSION=sde.DEFAULT)。SHAPE、MDB、GDB等传入路径。</param>
        /// <param name="workspaceType">工作空间类型</param>
        /// <returns></returns>
        public static IWorkspace GetWorkspace(string filePathOrConStr, enumWorkspaceType workspaceType)
        {
            IWorkspace wks = null;
            switch (workspaceType)
            {
                case enumWorkspaceType.GDB:
                    wks = GetGDBWorkspace(filePathOrConStr);
                    break;
                case enumWorkspaceType.MDB:
                    wks = GetMDBWorkspace(filePathOrConStr);
                    break;
                case enumWorkspaceType.SHAPE:
                    wks = GetSHAPEWorkspace(filePathOrConStr);
                    break;
                case enumWorkspaceType.SDE:
                    wks = GetSDEWorkspace(filePathOrConStr);
                    break;
                default:
                    break;
            }
            return wks;
        }

        /// <summary>
        /// 打开要素工作空间
        /// </summary>
        /// <param name="filePathOrConStr">ORACLE 直连传入连接字符串(INSTANCE=sde:oracle11g:127.0.0.1/ORCL.LOCALDOMAIN;USER=sde;PASSWORD=sde;VERSION=sde.DEFAULT)。SHAPE、MDB、GDB等传入路径。</param>
        /// <param name="workspaceType">工作空间类型</param>
        /// <returns></returns>
        public static IFeatureWorkspace GetFeatureWorkspace(string filePathOrConStr, enumWorkspaceType workspaceType)
        {
            return GetWorkspace(filePathOrConStr, workspaceType) as IFeatureWorkspace;
        }

        public static IWorkspace GetSDEWorkspace(string connectionStr)
        {
            var wksFactory = CreateWorkspaceFactory(enumWorkspaceType.SDE);
            IWorkspace wks = null;
            try
            {
                wks = (wksFactory as IWorkspaceFactory2).OpenFromString(connectionStr, -1);
            }
            catch
            {
                throw;
            }
            finally
            {
                Releaser.AEComReleaser.Release(wksFactory);
            }
            return wks;
        }


        public static IWorkspace GetMDBWorkspace(string filePath)
        {
            var wksFactory = CreateWorkspaceFactory(enumWorkspaceType.MDB);
            IWorkspace wks = null;
            try
            {
                wks = wksFactory.OpenFromFile(filePath, -1);
            }
            catch
            {
                throw;
            }
            finally
            {
                Releaser.AEComReleaser.Release(wksFactory);
            }
            return wks;
        }


        public static IWorkspace GetGDBWorkspace(string filePath)
        {
            var wksFactory = CreateWorkspaceFactory(enumWorkspaceType.GDB);
            IWorkspace wks = null;
            try
            {
                wks = wksFactory.OpenFromFile(filePath, -1);
            }
            catch
            {
                throw;
            }
            finally
            {
                Releaser.AEComReleaser.Release(wksFactory);
            }
            return wks;
        }


        public static IWorkspace GetSHAPEWorkspace(string filePath)
        {
            var wksFactory = CreateWorkspaceFactory(enumWorkspaceType.SHAPE);
            IWorkspace wks = null;
            try
            {
                wks = wksFactory.OpenFromFile(filePath, -1);
            }
            catch
            {
                throw;
            }
            finally
            {
                Releaser.AEComReleaser.Release(wksFactory);
            }
            return wks;
        }

        public static IWorkspaceFactory CreateWorkspaceFactory(enumWorkspaceType wksType)
        {
            IWorkspaceFactory wksFactory = null;

            switch (wksType)
            {
                case enumWorkspaceType.MDB:
                    wksFactory = new AccessWorkspaceFactoryClass();
                    break;
                case enumWorkspaceType.GDB:
                    wksFactory = new FileGDBWorkspaceFactoryClass();
                    break;
                case enumWorkspaceType.SHAPE:
                    wksFactory = new ShapefileWorkspaceFactoryClass();
                    break;
                case enumWorkspaceType.SDE:
                    wksFactory = new SdeWorkspaceFactoryClass();
                    break;
                default:
                    break;
            }
            return wksFactory;
        }
    }
}
