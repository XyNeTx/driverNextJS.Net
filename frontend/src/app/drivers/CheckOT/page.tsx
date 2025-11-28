import TableDriverCheckOT from "@/components/TableDriverCheckOT";

export default async function CheckOTPage() {

    return (
        <div>
            <div className="flex justify-between">
                <div className="flex-1 mt-2 ms-2 mb-4 ">
                    <h3 className="text-2xl font-bold">เวลา เข้า-ออกงาน และเวลา OT</h3>
                </div>
            </div>

            <TableDriverCheckOT/>

        </div>
    );
}