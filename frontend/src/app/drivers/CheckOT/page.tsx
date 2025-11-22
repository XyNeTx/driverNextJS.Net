import { Table, TableBody, TableCaption, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import CallAxios from "@/utils/CallAxios";
import Swal from "@/utils/SweetAlert2";

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

export default async function CheckOTPage() {
    const data = await GetDriverOTTime<DriverOT_DTOs>();
    return (
        <div>
        <div className="flex justify-between">
            <div className="flex-1 mt-2 ms-2 mb-4 ">
                <h3 className="text-2xl font-bold">เวลา เข้า-ออกงาน และเวลา OT</h3>
            </div>
        </div>
        <div className="flex justify-center h-[400px] p-10 border border-gray-100 shadow-lg shadow-indigo-400 dark:border-gray-800 dark:shadow-gray-400">
                <Table>
                    <TableCaption className="mb-4">List of Check-In Check-Out and Overtime</TableCaption>
                    <TableHeader className="sticky top-0 bg-white dark:bg-black">
                        <TableRow>
                            <TableHead>เวลาเข้าจริง</TableHead>
                            <TableHead>เวลาออกจริง</TableHead>
                            <TableHead>เวลาเข้า</TableHead>
                            <TableHead>เวลาออก</TableHead>
                            <TableHead>ชั่วโมงทำงานปกติ</TableHead>
                            <TableHead>ชั่วโมง OT 1.5</TableHead>
                            <TableHead>ชั่วโมง OT 2.0</TableHead>
                            <TableHead>ชั่วโมง OT 3.0</TableHead>
                            <TableHead>ชั่วโมง OT ทั้งหมด</TableHead>
                            <TableHead>ค่าอาหารกลางวัน</TableHead>
                            <TableHead>ค่า Taxi</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody id="tableBody">
                        {data && data.map((each) => (
                            <TableRow key={each.CheckInReal}>
                                <TableCell>{each.CheckInReal}</TableCell>
                                <TableCell>{each.CheckOutReal}</TableCell>
                                <TableCell>{each.CheckInCal}</TableCell>
                                <TableCell>{each.CheckOutCal}</TableCell>
                                <TableCell>{each.WorkingHours}</TableCell>
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
        </div>
    );
}