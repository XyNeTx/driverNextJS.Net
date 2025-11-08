'use client'
import { redirect, RedirectType } from "next/navigation";
import { useEffect, useState } from "react";
import CallAxios from "./CallAxios";

export default function CheckAuthen(){
    const [fullName,setFullName] = useState<string>("");
        useEffect(() => {
            const fetchUser = async () => {
                const arrCookie = document.cookie.split(";");
                //console.log(arrCookie);
                const isProd = process.env.NODE_ENV === "production";
                if (isProd) {
                    try {
                        const userName = await CallAxios<string>({
                            method: "GET",
                            url: "/api/ReportOutSource/Authen",
                        });

                        if (userName) {
                            setFullName(userName);
                        } else {
                            redirect(process.env.NEXT_PUBLIC_PROD_LOGIN_LINK ?? "", RedirectType.replace);
                        }
                    } catch (err) {
                        console.error(err);
                        redirect(process.env.NEXT_PUBLIC_PROD_LOGIN_LINK ?? "", RedirectType.replace);
                    }
                } else {
                    setFullName("Sitthiporn Polmart");
                }
            };
            fetchUser();
        }, []);

    return fullName;
}