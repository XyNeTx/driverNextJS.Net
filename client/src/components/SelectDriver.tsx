import CallAxios from "@/utils/CallAxios";

interface Driver {
    id : number;
    driverName : string;
    employeeCode : string;
}


async function GetDriverName<Driver>() : Promise<Driver[]>{
    try{
        const response = await CallAxios<Driver[]>({
            method : 'GET',
            url : '/api/ReportOutSource/GetDriverName',
        });
        return response;
    }
    catch(error){
        console.error("GetDriverName Error: ", error);
        return [];
    }

}

export default async function SelectDriver() {
    const arrDriverName = await GetDriverName<Driver>();
    return (
        <>
        <select title="เลือกคนขับรถ" className="border border-gray-300 rounded-md p-2 mr-2">
                    <option> -- เลือกคนขับรถ -- </option>
                    {arrDriverName.length === 0 && <option>ไม่พบข้อมูล</option>}
                    {arrDriverName.map((driver) => (
                        <option key={driver.id} value={driver.employeeCode}>{driver.driverName}</option>
                    ))}
        </select>
        </>
    )
}