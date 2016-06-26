using Bingotao.Customer.BaseLib.Entity;
using Bingotao.Customer.GIS;
using Bingotao.Customer.GIS.Analysis;
using Bingotao.Customer.GIS.LicenseInitializer;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Bingotao.Customer.BaseLib;
using Oracle.ManagedDataAccess.Client;

namespace ConsoleTest
{
    class Program
    {

        static void Main(string[] args)
        {
            string s = @"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL.LOCALDOMAIN)));user id=sde;Password=sde;";

            string sql = @"with zb as(
select hydm,hymc,t.ssze_gy_mj*0.58 zbz from zb_wx_hy_nd t where zbnd='2009'),
dxqy as(
select zb.hydm,zb.hymc,t1.yddwsyh,t4.xzqdm,t2.sfgsqy,zb.zbz,
t3.fttdmj ydmj,t1.jzmj*ftbl jzmj,t2.jsxssr*t3.ftbl jsxssr,t2.ssze*t3.ftbl ssze,t2.ybyskjss*t3.ftbl ybyskjss,t1.cyry*t3.ftbl cyry
 from yddw_jbxx t1 
left join yddw_ndzb t2 on t1.yddwsyh=t2.yddwsyh
left join dkxx_yddw t3 on t1.yddwsyh=t3.yddwsyh
left join dkxx_jbxx t4 on t3.dksyh=t4.dksyh
left join zb on substr(t1.hydm,1,2)=zb.hydm
where t2.nf='2009' and sfgyqy='是' and ssze_mj<=zb.zbz),
gs as(
select xzqdm,hydm,count(*) qygs_gy,sum(case when sfgsqy='是' then 1 else 0 end) qygs_gygs
from (select distinct yddwsyh,sfgsqy,hydm,xzqdm from dxqy)
group by xzqdm,hydm),
zb2 as(
select t.*,ssze/ydmj ssze_mj,jsxssr/ydmj jsxssr_mj,ybyskjss/ydmj ybyskjss_mj,cyry/ydmj cyry_mj from (
select hydm,xzqdm,sum(ydmj)*0.0015 ydmj,sum(jzmj)*0.0001 jzmj,
sum(jsxssr) jsxssr,sum(ssze) ssze,
sum(ybyskjss) ybyskjss,round(sum(cyry),0) cyry from dxqy
group by hydm,xzqdm)t),
h as (
select * from(
select dict_code hydm,'（'||dict_code||'）'||dict_name hymc from gyyd_dict t where t.dict_type='国民经济行业分类代码'),
(select dict_code xzqdm,dict_name xzqmc,dict_index idx from gyyd_dict t where t.dict_type='行政区代码'
union
select N'3202',N'无锡市区',0 from dual))
select xzqdm,xzqmc,hydm,hymc,
nvl(qygs_gy, 0)zs,nvl(qygs_gygs, 0) gsqys,nvl(ydmj, 0) ydmj,nvl(jzmj, 0) jzmj,nvl(jsxssr, 0) jsxssr,
nvl(ssze, 0) ssze,nvl(ybyskjss, 0) ybyskjss,nvl(cyry, 0) cyry,nvl(jsxssr_mj, 0) jsxssr_mj,nvl(ssze_mj, 0) ssze_mj,
nvl(cyry_mj, 0) cyry_mj,nvl(zbz, 0) zbz from(
select h.*,zb.zbz,gs.qygs_gy,gs.qygs_gygs,
zb2.ydmj,zb2.jzmj,zb2.jsxssr,zb2.ssze,zb2.ybyskjss,zb2.cyry,zb2.jsxssr_mj,zb2.ssze_mj,zb2.cyry_mj
 from h
left join zb on h.hydm=zb.hydm
left join gs on h.hydm=gs.hydm and h.xzqdm=gs.xzqdm
left join zb2 on h.hydm=zb2.hydm and h.xzqdm=zb2.xzqdm
union
(select N'00'hydm,N'工业行业'hymc,N'3202'xzqdm,N'无锡市区'xzqmc,0 idx,0 zbz,tt1.*,tt2.* from(
select count(*) qygs_gy,sum(case when sfgsqy='是' then 1 else 0 end )gygs_gygs
from dxqy)tt1,(
select t.*,jsxssr/ydmj jsxssr_mj,ssze/ydmj ssze_mj,cyry/ydmj cyry_mj from(
select sum(ydmj)*0.0015 ydmj,sum(jzmj)*0.0001 jzmj,sum(jsxssr)jsxssr,sum(ssze)ssze,sum(ybyskjss)ybyskjss,round(sum(cyry)) cyry
  from dxqy)t)tt2)) where hydm in ('06','07','08','09','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','28','29','30','31','32','33','34','35','36','37','38','39','40','41','42','43','44','45','46','00') and xzqdm in ('320205','320206','320211','320292','320202','320203','320204','3202','0000')
  order by hydm,idx";

            OracleDataAdapter da = new OracleDataAdapter(sql, s);
            DataTable dt = new DataTable();
            da.Fill(dt);


            //var dict = Analysis.DoAnalysis("1");
            //string s = Newtonsoft.Json.JsonConvert.SerializeObject(dict, new CustomDoubleConverter());
        }
    }


}
