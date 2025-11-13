'use client'
import CallAxios from "@/utils/CallAxios";
import { toast } from "sonner";
import { Button } from "./ui/button";
import { unApproveList } from "./TableShowWaitingData";

const toastPromise = () => 
    toast.promise(ApproveAll(), {
        loading: 'Approving Data . . .',
        success: () => 'All Data was Approved', //if didn't have any exception will show success toast
        error: () => 'Can not Approve All Data' //if catch an exception will show error toast
    });

const delay = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

const ApproveAll = async ():Promise<void> => {
    return console.log(unApproveList);
    try{
        await delay(3000);
        await CallAxios<void>({
            method :'POST',
            url:'/api/ReportOutSource/ApproveAllWaitingData',
            //data: unApproveList,
        });
    }
    catch (err){
        console.log({err});
        throw err;
    }
}

function UserConfirm(){
    toast.warning("Are you sure to Approve All Data in Table",{
        description: 'This Action cant be Undone',
        action:{
            label: 'Yes',
            onClick: (e) => {
                if(e){
                    toastPromise();
                }
            }
        }
    })
}

export default function ButtonApproveAll() {
    return (
        <div className="mt-2 ms-2 mb-4 ">
            <Button variant="outline" onClick={UserConfirm}>Approve All Data</Button>
        </div>
    );
}