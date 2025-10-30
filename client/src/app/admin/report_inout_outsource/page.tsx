'use client'
import SelectDriver from "@/components/SelectDriver";
import SelectYear from "@/components/SelectYear";
import SelectMonth from "@/components/SelectMonth";
import { useState } from "react";
import TableReportOutSource, { VM_Driver_Outsource_Report } from "@/components/TableReportOutSource";
import showSwal from "@/utils/SweetAlert2";
import CallAxios from "@/utils/CallAxios";


export interface VM_CallReport {
    EmployeeCode : string,
    Year : string,
    Month : string
};


async function GetReportData<VM_Driver_Outsource_Report>(_VM : VM_CallReport) : Promise<VM_Driver_Outsource_Report>{
    try{
        const response:VM_Driver_Outsource_Report = await CallAxios<VM_Driver_Outsource_Report>({
            method: 'GET',
            url: '/api/ReportOutSource/GetReportDriverOutSource',
            params:_VM
        });

        return response;
    }
    catch (error){
        console.error(error);
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
            return showSwal({
                icon : "error",
                title : "Invalid Input",
                text : "กรุณาเลือก คนขับ เดือน และ ปี ให้ครบก่อน"
            })
        }
        setLoading(true);
        const _VM : VM_CallReport = {
            EmployeeCode : empCode,
            Year : year,
            Month : month
        }
        const data = await GetReportData<VM_Driver_Outsource_Report>(_VM)
        setReportData(data);

        setLoading(false);
    }

    return (
        <div className="p-2">
            <h3 className="font-bold text-2xl">ค้นหาข้อมูลคนขับรถ</h3>
            <div className="flex-row-reverse mt-4 mb-4 flex">
                <div className="ps-2">
                    <button className="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded">
                        อัพเดทและค้นหา
                    </button>
                </div>
                <div>
                    <button className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
                        onClick={OnBtnClicked}>
                        {loading ? 'กำลังค้นหา...' : 'ค้นหา'}
                    </button>
                </div>
                <SelectDriver value={empCode} onChange={setEmpCode} />
                <SelectMonth  value={month} onChange={setMonth} />
                <SelectYear  value={year} onChange={setYear} />
            </div>

            <div className="block mt-4 mb-4">
                ตารางแสดงข้อมูลคนขับรถ

                <TableReportOutSource reportData={reportData}/>

            </div>

            <div className="block text-red-600 text-sm">
                <p>Job Type Describe </p>
                <p>1 : In Working Day Out in Same Day</p>
                <p>2 : In Holiday Out in Same Day</p>
                <p>3 : In Working Day Out in Holiday</p>
                <p>4 : In Holiday Out in Working Day</p>
                <p>5 : In Working Day Out in Another Working Day</p>
                <p>6 : In Holiday Out in Another Holiday</p>
            </div>

        </div>
    )
};