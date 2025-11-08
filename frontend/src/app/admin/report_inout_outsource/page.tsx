'use client'
import SelectDriver from "@/components/SelectDriver";
import SelectYear from "@/components/SelectYear";
import SelectMonth from "@/components/SelectMonth";
import { Suspense, useState } from "react";
import TableReportOutSource, { VM_Driver_Outsource_Report } from "@/components/TableReportOutSource";
import Swal from "@/utils/SweetAlert2";
import Axios from "@/utils/CallAxios";


export interface VM_CallReport {
    EmployeeCode : string,
    Year : string,
    Month : string
};


async function GetReportData<VM_Driver_Outsource_Report>(_VM : VM_CallReport) : Promise<VM_Driver_Outsource_Report>{
    try{
        const response:VM_Driver_Outsource_Report = await Axios<VM_Driver_Outsource_Report>({
            method: 'GET',
            url: '/api/ReportOutSource/GetReportDriverOutSource',
            params:_VM,

        });

        return response;
    }
    catch (error){
        console.error(error);
            Swal({
            icon:'error',
            title: "Error",
            text: "Can't Get Report Driver Outsource"
        })
        return {} as VM_Driver_Outsource_Report;
    }
}

async function RefreshReportData<VM_Driver_Outsource_Report>(_VM : VM_CallReport) : Promise<VM_Driver_Outsource_Report>{
    try{
        const response:VM_Driver_Outsource_Report = await Axios<VM_Driver_Outsource_Report>({
            method: 'GET',
            url: '/api/ReportOutSource/RefreshReportDriverOutSource',
            params:_VM,
        });

        return response;
    }
    catch (error){
        console.error(error);
            Swal({
            icon:'error',
            title: "Error",
            text: "Can't Get Report Driver Outsource"
        })
        return {} as VM_Driver_Outsource_Report;
    }
}


export default function ReportInOutOutSource(){
    const [empCode,setEmpCode] = useState<string>("");
    const [month,setMonth] = useState<string>("");
    const [year,setYear] = useState<string>("");
    const [reportData,setReportData] = useState<VM_Driver_Outsource_Report>({} as VM_Driver_Outsource_Report);
    const [loading,setLoading] =useState<boolean>(false);

    const OnBtnClicked = async () => {
        if (!empCode || !month || !year){
            return Swal({
                icon : "error",
                title : "Invalid Input",
                text : "กรุณาเลือก คนขับ เดือน และ ปี ให้ครบก่อน"
            })
        }
        setLoading(true);
        setReportData({} as VM_Driver_Outsource_Report);
        const _VM : VM_CallReport = {
            EmployeeCode : empCode,
            Year : year,
            Month : month
        }
        const data = await GetReportData<VM_Driver_Outsource_Report>(_VM)
        setReportData(data);

        setLoading(false);
    }

    const RefreshClicked = async () => {
        if (!empCode || !month || !year){
            return Swal({
                icon : "error",
                title : "Invalid Input",
                text : "กรุณาเลือก คนขับ เดือน และ ปี ให้ครบก่อน"
            })
        }
        setLoading(true);
        setReportData({} as VM_Driver_Outsource_Report);
        const _VM : VM_CallReport = {
            EmployeeCode : empCode,
            Year : year,
            Month : month
        }
        const data = await RefreshReportData<VM_Driver_Outsource_Report>(_VM)
        setReportData(data);

        setLoading(false);
    }

    return (
        <div className="p-2">
            <h3 className="font-bold text-2xl">ค้นหาข้อมูลคนขับรถ</h3>
            <div className="flex-row-reverse mt-4 mb-4 flex">
                <div className="ps-2">
                    <button className="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded" onClick={RefreshClicked}>
                        {loading ? 'คำนวณข้อมูล . . .' : 'อัพเดทและค้นหา'}
                    </button>
                </div>
                <div>
                    <button className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
                        onClick={OnBtnClicked}>
                        {loading ? 'กำลังค้นหา...' : 'ค้นหา'}
                    </button>
                </div>
                <Suspense fallback={<p>กำลังโหลดข้อมูลรายชื่อคนขับรถ</p>}>

                    <SelectDriver value={empCode} onChange={setEmpCode} />
                    <SelectMonth  value={month} onChange={setMonth} />
                    <SelectYear  value={year} onChange={setYear} />

                </Suspense>
            </div>

            <div className="block mt-4 mb-4">
                ตารางแสดงข้อมูลคนขับรถ
            </div>

            <TableReportOutSource reportData={reportData}/>

            <div className="block text-red-600 text-sm mt-4">
                <p>Job Type Describe </p>
                <p>1 : In Working Day Out Same Day</p>
                <p>2 : In Holiday Out Same Day</p>
                <p>3 : In Working Day Out Holiday</p>
                <p>4 : In Holiday Out Working Day</p>
                <p>5 : In Working Day Out Another Working Day</p>
                <p>6 : In Holiday Out Another Holiday</p>
            </div>

        </div>
    )
};