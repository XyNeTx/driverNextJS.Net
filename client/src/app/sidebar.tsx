'use client'
import AppImage from "@/components/AppImage";
import Swal from "@/utils/SweetAlert2";
import axios from "axios";
import { usePathname } from "next/navigation";
import { pageTitle } from "@/constants/pageTitle";
import { useMemo } from "react";
import ThemeToggle from "@/components/ToggleTheme";
import Lineicons from "@lineiconshq/react-lineicons";
import { BarChart4Outlined, Crown3Outlined, Home2Outlined, MercedesOutlined, Notebook1Outlined } from "@lineiconshq/free-icons";
import CheckAuthen from "@/utils/CheckAuthen";

const Logout = async () => {

    await Swal({
        title: "ออกจากระบบ",
        text: "ยืนยัน!",
        icon: "info",
    })
    .then(function (isConfirm)
    {
        //console.log(isConfirm);
        const formData = new FormData();
        formData.append('Controller','Logout');
        const Service_Link = process.env.NEXT_PUBLIC_PROD_SERVICE_LINK ?? ""
        if(Service_Link === ""){
            return;
        }
        if (isConfirm == true) {
            axios.post(Service_Link, formData)
            .then(function (response) {
                console.log(response);
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
    const pathname = usePathname();
    const userName = CheckAuthen();

    // Find the last segment
    const lastSegment = useMemo(() => pathname.split("/").pop() ?? "", [pathname]);

    // Find the matching title
    const currentPage = useMemo(
        () =>
        pageTitle.find(
            (p) => p.urlName.toLowerCase() === lastSegment.toLowerCase()
        ),
        [lastSegment]
    );

    return (
        <>
        <div className="p-2 fixed w-full bg-indigo-900 text-white dark:bg-black dark:text-white">
            <div className="ml-4 flex flex-row items-center justify-between w-full">
                <div>
                    <a href="https://hmmtweb01.hinothailand.com/Drivers/Home.aspx" className="logo">
                        <AppImage src="/bus-driver.png" alt="Logo" width={50} height={50} />
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
                <div className="ps-6">
                    {currentPage &&
                        (
                            <h3 className="font-bold text-lg">{currentPage.name}</h3>
                        )
                    }
                </div>
                <ThemeToggle/>
                <div className="ms-2 mr-8">
                    <a href="#" onClick={Logout} className="btn-logout font-bold">{userName}</a>
                </div>
            </div>

            <aside className="w-48 h-[calc(100vh-40px)] lg:fixed top-16.5 left-0 p-2 pt-4 border-r bg-indigo-900 text-white dark:bg-black dark:text-white">
                <nav className="flex flex-col gap-4 ">
                    <ul className="">
                        <li className="flex flex-row">
                            <div className="flex-1">
                                <a className="nav-link" href="https://hmmtweb01.hinothailand.com/Drivers/Home.aspx">
                                    <Lineicons icon={Home2Outlined} className="nav-link ms-0 mt-1 mb-1 me-1" size={24} />
                                    Home
                                </a>
                            </div>
                        </li>
                    </ul>
                    <ul className="">
                        <li className="flex flex-row">
                            <div className="flex-1">
                                <a className="nav-link" href="https://hmmtweb01.hinothailand.com/Drivers/Driver_ListDriver.aspx">
                                    <Lineicons icon={MercedesOutlined} className="nav-link ms-0 mt-1 mb-1 me-1" size={24} />
                                    Driver
                                </a>
                            </div>
                        </li>
                    </ul>
                    <ul className="">
                        <li className="flex flex-row">
                            <div className="flex-1">
                                <a className="nav-link" href="https://hmmtweb01.hinothailand.com/Drivers/Boss_listboss.aspx">
                                    <Lineicons icon={Crown3Outlined} className="nav-link ms-0 mt-1 mb-1 me-1" size={24} />
                                    Boss
                                </a>
                            </div>
                        </li>
                    </ul>
                    <ul className="">
                        <li className="flex flex-row">
                            <div className="flex-1">
                                <a className="nav-link" href="https://hmmtweb01.hinothailand.com/Drivers/Admin_MenuDriver.aspx">
                                    <Lineicons icon={BarChart4Outlined} className="nav-link ms-0 mt-1 mb-1 me-1" size={24} />
                                    Report Driver
                                </a>
                            </div>
                        </li>
                    </ul>
                    <ul className="">
                        <li className="flex flex-row">
                            <div className="flex-1">
                                <a className="nav-link" href="https://hmmtweb01.hinothailand.com/Drivers/Admin_MenuBoss.aspx">
                                    <Lineicons icon={Notebook1Outlined} className="nav-link ms-0 mt-1 mb-1 me-1" size={24} />
                                    Report Boss
                                </a>
                            </div>
                        </li>
                    </ul>
                </nav>
            </aside>

        </div>
        </>
    );
};

