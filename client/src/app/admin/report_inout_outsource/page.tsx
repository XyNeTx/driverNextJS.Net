import SelectDriver from "@/components/SelectDriver";
import SelectYear from "@/components/SelectYear";
import SelectMonth from "@/components/SelectMonth";
import { Metadata } from "next";
import { pageTitle } from "@/constants/pageTitle";
import { useState } from "react";

export async function generateMetadata(): Promise<Metadata> {
    const titlePage = pageTitle.find(
        (p) => p.urlName.toLowerCase() === 'report_inout_outsource'
    )
    return {
        title: titlePage ? titlePage.title : 'Driver Tracking',
    };
}


export default async function ReportInOutOutSource(){
    return (
        <div className="p-2 ">
            <h3 className="font-bold text-2xl">ค้นหาข้อมูลคนขับรถ</h3>

            <div className="flex-row-reverse mt-4 mb-4 flex">
                <button className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                    ค้นหา
                </button>
                <SelectDriver />
                <SelectMonth />
                <SelectYear />
            </div>

            <div className="block mt-4 mb-4">
                ตารางแสดงข้อมูลคนขับรถ
                <div className="overflow-x-auto">
                    <table className="table-auto border-collapse border border-gray-400 mt-4 w-full">
                        <thead >
                            <tr>
                                <th className="thReport1stRow" rowSpan={3}>Check IN</th>
                                <th className="thReport1stRow" rowSpan={3}>Check Out</th>
                                <th className="thReport1stRow" rowSpan={3}>Job Type</th>
                                <th className="thReport1stRow" rowSpan={3}>Temp Drive</th>
                                <th className="thReport1stRow" rowSpan={3}>Use/No Use</th>
                                <th className="thReport1stRow" rowSpan={3}>Cal Time IN</th>
                                <th className="thReport1stRow" rowSpan={3}>Cal Time OUT</th>
                                <th className="thReport1stRow" colSpan={5}>Working Day</th>
                                <th className="thReport1stRow" colSpan={4}>Holiday</th>
                                <th className="thReport1stRow" rowSpan={3}>Total OT</th>
                                <th className="thReport1stRow" rowSpan={3}>Taxi</th>
                                <th className="thReport1stRow" rowSpan={3}>Lunch</th>
                            </tr>
                            <tr>
                                <th className="thReport2ndRow" >OT1.5</th>
                                <th className="thReport2ndRow" >Regular Time</th>
                                <th className="thReport2ndRow" >OT1.5</th>
                                <th className="thReport2ndRow" >OT2.0</th>
                                <th className="thReport2ndRow" rowSpan={2} >Total OT</th>
                                <th className="thReport2ndRow" >OT3.0</th>
                                <th className="thReport2ndRow" >OT2.0</th>
                                <th className="thReport2ndRow" >OT3.0</th>
                                <th className="thReport2ndRow" rowSpan={2} >Total OT</th>
                            </tr>
                            <tr>
                                <th className="thReport3rdRow" >00:01-7:29</th>
                                <th className="thReport3rdRow" >07:30-16:30</th>
                                <th className="thReport3rdRow" >16:31-22:00</th>
                                <th className="thReport3rdRow" >22:01-00:00</th>
                                <th className="thReport3rdRow" >00:01-07:29</th>
                                <th className="thReport3rdRow" >07:30-16:30</th>
                                <th className="thReport3rdRow" >16:31-00:00</th>
                            </tr>
                        </thead>
                    </table>
                </div>

            </div>

        </div>
    )
};