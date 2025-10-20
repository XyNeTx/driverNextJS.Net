'use client'
import Image from "next/image";
import { usePathname } from "next/navigation";
import axios from "axios";
import swAlert from  "../components/sweetalert2";

const pageName = [
    {
        name: "ตารางเข้าออกงานคนขับรถ (Outsource)",
        urlName: "report_inout_outsource"
    }
]
const Logout = () => {
    return swAlert({
        title: "ออกจากระบบ",
        text: "ยืนยัน!",
        icon: "info",
    }).then(function (isConfirm : any) {
        if (isConfirm.isConfirmed == true) {
            axios.post("https://hmmtweb01.hinothailand.com/Drivers/Service.aspx", { Controller: "Logout" },
                { headers:
                    {
                        "Content-Type": "application/json",
                        "Access-Control-Allow-Origin": "*",
                    },
                    withCredentials: true
                }
            )
                .then(function (response) {
                    if (response) {
                        document.cookie = "BROWSERID= ; expires = Thu, 01 Jan 1970 00:00:00 GMT"
                        document.cookie = "BROWSERCURRENT= ; expires = Thu, 01 Jan 1970 00:00:00 GMT"
                        document.cookie = "BROWSERDEVICES= ; expires = Thu, 01 Jan 1970 00:00:00 GMT"
                        document.cookie = "BROWSERACCESS= ; expires = Thu, 01 Jan 1970 00:00:00 GMT"
                        location.reload();
                    }
                });
        }
    }
    );
}

const toggleSidebar = () => {
    const sidebar = document.querySelector("aside");
    const main = document.querySelector("main");
    if (sidebar) {
        sidebar.classList.toggle("hidden");
        main?.classList.toggle("pl-52");
    }
};


export default function Sidebar() {
    const pathname = usePathname(); // e.g. "/RequestApprove"
    //console.log(pathname);
    const lastSegment = pathname.split("/").pop() ?? "not found";
    //console.log(lastSegment);

    const currentPage = pageName.find(
        (p) => p.urlName.toLowerCase() === lastSegment.toLowerCase()
    );

    return (
        <>
        <div className="p-2 bg-white shadow-md text-sky-600">
            <div className="ml-4 flex flex-row items-center justify-between w-full">
                {/* <div className="flex flex-row p-2 bg-white border-b font-black text-black"></div> */}
                <div className="">
                    <a href="https://hmmtweb01.hinothailand.com/Drivers/Home.aspx" className="logo">
                        <Image src="/bus-driver.png" alt="Logo" width={75} height={75} />
                        <span className="sr-only">Driver NextJS</span>
                    </a>
                </div>
                <div className="pl-6 pr-6">
                    <h3 className="col font-bold text-lg">Driver Tracking</h3>
                </div>
                <div>
                    <a title="toggleSidebar" href="#" onClick={toggleSidebar} >
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="size-6">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" />
                        </svg>
                    </a>
                </div>
                <div className="flex-1 ps-6">
                    {currentPage && (
                        <h3 className="font-bold text-lg">{currentPage.name}</h3>
                    )}
                </div>
                <div className="mr-8">

                    <a href="#" onClick={Logout} className="btn-logout font-bold">Sitthiporn Polmart</a>
                </div>
            </div>

            <aside className="w-48 h-[calc(100vh-96px)] fixed top-24 left-0 p-4 border-r bg-white shadow-md text-sky-600">
                <nav className="flex flex-col gap-4">
                    <ul className="mb-4 border-b pb-2">
                        <li className="pl-4 pr-4">
                            <a className="font-semibold" href="https://hmmtweb01.hinothailand.com/Drivers/Home.aspx">หน้าแรก</a>
                        </li>
                    </ul>
                    <ul className="mb-4 border-b pb-2">
                        <li className="pl-4 pr-4">
                            <a className="font-semibold" href="https://hmmtweb01.hinothailand.com/Drivers/Driver_InOutMain.aspx">คนขับรถ</a>
                        </li>
                    </ul>
                </nav>
            </aside>

        </div>
        </>
    );
};

