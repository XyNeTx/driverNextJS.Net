'use client'
import AppImage from "@/components/AppImage";
import swAlert from "@/utils/SweetAlert2";
import axios from "axios";
import { usePathname } from "next/navigation";
import { pageTitle } from "@/constants/pageTitle";

const Logout = async () => {

    await swAlert({
        title: "ออกจากระบบ",
        text: "ยืนยัน!",
        icon: "info",
    })
    .then(function (isConfirm)
    {
        //console.log(isConfirm);
        if (isConfirm == true) {
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
        main?.classList.toggle("sidebar-hidden");
    }
};



export default function Sidebar() {
    const pathname = usePathname(); // e.g. "/RequestApprove"
    //console.log(pathname);
    const lastSegment = pathname.split("/").pop() ?? "not found";
    //console.log(lastSegment);

    const currentPage = pageTitle.find(
        (p) => p.urlName.toLowerCase() === lastSegment.toLowerCase()
    );

    return (
        <>
        <div className="p-2">
            <div className="nav-header">
                <div>
                    <a href="https://hmmtweb01.hinothailand.com/Drivers/Home.aspx" className="logo">
                        <AppImage src="/bus-driver.png" alt="Logo" width={75} height={75} />
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
                    {currentPage &&
                        (
                            <h3 className="font-bold text-lg">{currentPage.name}</h3>
                        )
                    }
                </div>
                <div className="mr-8">
                    <a href="#" onClick={Logout} className="btn-logout font-bold">Sitthiporn Polmart</a>
                </div>
            </div>

            <aside>
                <nav className="flex flex-col gap-4">
                    <ul className="border-b">
                        <li>
                            <a className="nav-link" href="https://hmmtweb01.hinothailand.com/Drivers/Home.aspx">หน้าแรก</a>
                        </li>
                    </ul>
                    <ul className="border-b">
                        <li>
                            <a className="nav-link" href="https://hmmtweb01.hinothailand.com/Drivers/Driver_InOutMain.aspx">คนขับรถ</a>
                        </li>
                    </ul>
                </nav>
            </aside>

        </div>
        </>
    );
};

