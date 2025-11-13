import ButtonApproveAll from "@/components/ButtonApproveAll";
import TableShowWaitingData from "@/components/TableShowWaitingData";

export default function ApproveAllPage() {
    return (
        <div className="">
            <div className="flex justify-between">
                <div className="flex-1 mt-2 ms-2 mb-4 ">
                    <h3 className="text-2xl font-bold">Approve All Request Data</h3>
                </div>
                <ButtonApproveAll/>
            </div>
            <TableShowWaitingData/>
        </div>
    );
}