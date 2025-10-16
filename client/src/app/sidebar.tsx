'use client'
import Image from "next/image";
import { usePathname } from "next/navigation";
import { useState,useEffect } from "react";

const pageName = [
    {
        name: "เข้างาน / ออกงาน",
        urlName: "RequestApprove"
    }
]
// const Logout = () => {
//     if (confirm("คุณต้องการออกจากระบบใช่หรือไม่?")) {
//         window.location.href = "https://hmmtweb01.hinothailand.com/Drivers/Home.aspx";
//     }
// };

export default function Sidebar() {
    const pathname = usePathname(); // e.g. "/RequestApprove"
    //console.log(pathname);
    const lastSegment = pathname.split("/").pop() ?? "not found";
    //console.log(lastSegment);

    const currentPage = pageName.find(
        (p) => p.urlName.toLowerCase() === lastSegment.toLowerCase()
    );

    const [mounted, setMounted] = useState(false);

    useEffect(() => {
        setMounted(true);
    }, []);

    if (!mounted) {
        // prevent mismatch: render nothing or a placeholder during SSR
        return null;
    }

    console.log(currentPage);

    return (
        <>
        <div className="p-2">

            <div className="flex flex-row">
                {/* <div className="flex flex-row p-2 bg-white border-b font-black text-black"></div> */}
                <div className="">
                    <a href="https://hmmtweb01.hinothailand.com/Drivers/Home.aspx" className="logo">
                        <Image src="/bus-driver.png" alt="Logo" width={75} height={75} />
                        <span className="sr-only">Driver NextJS</span>
                    </a>
                </div>
                <div className="w-40 content-center ps-6">
                    <h3 className="col font-bold text-lg">Driver Tracking</h3>
                </div>
                <div className="w-100 content-center ps-6">
                    {currentPage && (
                        <h3 className="col font-bold text-lg">{currentPage.name}</h3>
                    )}
                    <h3 className="col font-bold text-lg"></h3>
                </div>
                <div className="content-center pe-2 ">
                    <a href="#" className="btn-logout">Logout</a>
                </div>
            </div>


            <nav className="flex flex-col">
                <ul>
                    <li>
                        <a href="https://hmmtweb01.hinothailand.com/Drivers/Home.aspx">หน้าแรก</a>
                    </li>
                </ul>
                <ul>
                    <li>
                        <a href="https://hmmtweb01.hinothailand.com/Drivers/Driver_InOutMain.aspx">คนขับรถ</a>
                    </li>
                </ul>
            </nav>

        </div>
        </>
    );
};

