import CallAxios from "@/utils/CallAxios";
import { Table, TableBody, TableCaption, TableCell, TableFooter, TableHead, TableHeader, TableRow } from "./ui/table";
import ButtonApproveAll from "./ButtonApproveAll";

interface Driver_TimeAttendanceDTO
{
    Time_Id : number;
    EmployeeCode : string;
    EmployeeFullName :string;
    CheckIn : string;
    CheckOut : string
    WorkTypeIn : string
    WorkTypeOut: string
    BossID: string
    BossFullName: string
}

async function GetWaitingData() : Promise<Driver_TimeAttendanceDTO[]>{
    try{
        const data = await CallAxios<Driver_TimeAttendanceDTO[]>({
            method: 'GET',
            url : '/api/ReportOutSource/GetAllAttendanceWaitingData',
        })
        return data ?? [];
    }
    catch (err){
        console.error(err);
        return [];
    }
}

export default async function TableShowWaitingData() {
    const data = await GetWaitingData();
    //console.log({data});
    //console.log(unApproveList);
    return (
        <>
            <div className="flex justify-center h-[400px] p-10 border border-gray-100 shadow-lg shadow-indigo-400 dark:border-gray-800">
                <Table>
                    <TableCaption className="mb-4">A list of your waiting request data</TableCaption>
                    <TableHeader>
                        <TableRow>
                            <TableHead>Employeee Code</TableHead>
                            <TableHead className="w-[100px]">Full Name</TableHead>
                            <TableHead>Time In</TableHead>
                            <TableHead>Time Out</TableHead>
                            <TableHead>Work In</TableHead>
                            <TableHead>Work Out</TableHead>
                            <TableHead>Boss Code</TableHead>
                            <TableHead>Boss Name</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody id="tableBody">
                        {data && data.map((each) => (
                            <TableRow className="cls_Time_Id" data-key={each.Time_Id} key={each.Time_Id}>
                                <TableCell>{each.EmployeeCode}</TableCell>
                                <TableCell className="w-[100px]">{each.EmployeeFullName}</TableCell>
                                <TableCell>{each.CheckIn.replace("T"," ")}</TableCell>
                                <TableCell>{each.CheckOut.replace("T"," ")}</TableCell>
                                <TableCell>{each.WorkTypeIn}</TableCell>
                                <TableCell>{each.WorkTypeOut}</TableCell>
                                <TableCell>{each.BossID}</TableCell>
                                <TableCell>{each.BossFullName}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                    {/* <TableFooter>
                        <TableRow>
                            <TableCell colSpan={3}>Total</TableCell>
                            <TableCell className="text-right">$2,500.00</TableCell>
                            </TableRow>
                            </TableFooter> */}
                </Table>
            </div>
        </>
    );
}