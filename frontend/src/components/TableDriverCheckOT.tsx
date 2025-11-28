'use client'
import { Table, TableBody, TableCaption, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import CallAxios from "@/utils/CallAxios";
import Swal from "@/utils/SweetAlert2";
import { useEffect, useState } from "react";

interface DriverOT_DTOs
{
    CheckInReal :string
    CheckOutReal :string
    CheckInCal :string
    CheckOutCal :string
    WorkingHours :string
    SumOT_1_5 :string
    SumOT_2_0 :string
    SumOT_3_0 :string
    SumOT :string
    Lunch :number
    Taxi:number
}

async function GetDriverOTTime<DriverOT_DTOs>() : Promise<DriverOT_DTOs[]>{
    try{
        const response : DriverOT_DTOs[] = await CallAxios<DriverOT_DTOs[]>({
            method: 'GET',
            url: '/api/ReportOutSource/GetDriverOTTime',
        });
        //console.log(response);
        return response;
    }
    catch (error){
        console.error(error);
            Swal({
            icon:'error',
            title: "Error",
            text: "Can't Get Report Driver Outsource"
        })
        return [];
    }
}

export default function TableDriverCheckOT() {
    const [data,setdata] = useState<DriverOT_DTOs[]>([])
    useEffect(()=>{
            const fetchOT = async () => {
                const fetchedData = await GetDriverOTTime<DriverOT_DTOs>();
                setdata(fetchedData);
            }
            fetchOT();
        },[]
    )
    return (
        <div className="flex overflow-y-scroll lg:max-h-[400px] not-lg:max-h-[400px] lg:p-10 not-lg:p-2 border border-gray-100 shadow-lg shadow-indigo-400 dark:border-gray-800 dark:shadow-gray-400">
                <Table className="table-auto p-0">
                    <TableCaption className="mb-4">List of Check-In Check-Out and Overtime</TableCaption>
                    <TableHeader className="sticky top-0 bg-white dark:bg-black">
                        <TableRow>
                            {/* <TableHead>เวลาเข้าจริง</TableHead>
                            <TableHead>เวลาออกจริง</TableHead> */}
                            <TableHead>เวลาเข้า</TableHead>
                            <TableHead>เวลาออก</TableHead>
                            {/* <TableHead>ชั่วโมงทำงานปกติ</TableHead> */}
                            <TableHead>OT 1.5</TableHead>
                            <TableHead>OT 2.0</TableHead>
                            <TableHead>OT 3.0</TableHead>
                            <TableHead>SUM OT</TableHead>
                            <TableHead>Taxi</TableHead>
                            <TableHead>Lunch</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody className=" bg-white dark:bg-black" id="tableBody">
                        {data && data.map((each) => (
                            <TableRow key={each.CheckInReal}>
                                {/* <TableCell>{each.CheckInReal}</TableCell>
                                <TableCell>{each.CheckOutReal}</TableCell> */}
                                <TableCell>{each.CheckInCal}</TableCell>
                                <TableCell>{each.CheckOutCal}</TableCell>
                                {/* <TableCell>{each.WorkingHours}</TableCell> */}
                                <TableCell>{each.SumOT_1_5}</TableCell>
                                <TableCell>{each.SumOT_2_0}</TableCell>
                                <TableCell>{each.SumOT_3_0}</TableCell>
                                <TableCell>{each.SumOT}</TableCell>
                                <TableCell>{each.Taxi * 125}</TableCell>
                                <TableCell>{each.Lunch * 50}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </div>
    );
}