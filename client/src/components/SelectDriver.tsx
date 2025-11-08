'use client'
import Axios from "@/utils/CallAxios";
import { useEffect,useState } from "react";

interface Driver {
    ID : number;
    DriverName : string;
    EmployeeCode : string;
}

async function GetDriverName() : Promise<Driver[]>{
    try
    {
        const response = await Axios<Driver[]>({
            method : 'GET',
            url : '/api/ReportOutSource/GetDriverName',
        });
        //console.log(response);
        return response;
    }
    catch(error){
        console.error("GetDriverName Error: ", error);
        return [];
    }

}

interface SelectProps {
    value: string;
    onChange: (val: string) => void;
}

export default function SelectDriver({value,onChange}: SelectProps) {
    const [arrDriverName,setArrDriverName] = useState<Driver[]>([]);

    useEffect(()=> {
        (async () =>
            {
                const data :Driver[] = await GetDriverName();
                console.log(data);
                setArrDriverName(data);
            }
        )();
    },[]);

    return (
        <>
        <select
            value={value}
            onChange={(e)=> onChange(e.target.value)}
            title="เลือกคนขับรถ" className="border border-gray-300 rounded-md p-2 mr-2">
                <option> -- เลือกคนขับรถ -- </option>
                {arrDriverName.length === 0 && <option>ไม่พบข้อมูล</option>}
                {arrDriverName.map((driver) => (
                    <option key={driver.ID} value={driver.EmployeeCode}>{driver.DriverName}</option>
                ))}
        </select>
        </>
    )
}