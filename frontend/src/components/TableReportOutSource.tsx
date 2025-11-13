export interface VM_Driver_Outsource_Report{
    ReportList : Driver_OutSource[],
    ReferReport : VM_Total_Report_Outsource,
}

export interface Driver_OutSource {
    ID : number,
    EmployeeCode : string,
    Check_In : string,
    Check_Out : string,
    Job_Type : number,
    Temp_Drive :string,
    Use_NoUse : string,
    Cal_Time_In:string
    Cal_Time_Out:string
    Work_OT1_5_Night:string
    Work_Reg:string
    Work_OT1_5_Eve:string
    Work_OT2:string
    Work_Total_OT:string
    Holi_OT3_0:string
    Holi_OT2_0:string
    Holi_OT3_0_Eve:string
    Holi_Total_OT:string
    All_Total_OT :string
    Taxi : number
    Lunch : number
}

export interface VM_Total_Report_Outsource {
    TotalWork_OT1_5_Night :string
    TotalWork_Reg :string
    TotalWork_OT1_5_Eve: string
    TotalWork_OT2 :string
    TotalWork_Total_OT :string
    TotalHoli_OT3_0 :string
    TotalHoli_OT2_0 :string
    TotalHoli_OT3_0_Eve :string
    TotalHoli_Total_OT :string
    TotalAll_Total_OT :string
    TotalTaxi :number
    TotalLunch :number
}
export default function TableReportOutSource({reportData}: {reportData : VM_Driver_Outsource_Report}){
    return (
        <>
            <div className="flex max-h-200 overflow-y-scroll border border-gray-400 border-l-4 rounded-l-xl border-l-red-400 dark:border-white" >
            <table className="table-auto">
                <thead className="sticky top-0 z-10 bg-white dark:bg-black">
                    <tr>
                        <th className="thReport1stRow" rowSpan={3}>Check IN</th>
                        <th className="thReport1stRow" rowSpan={3}>Check Out</th>
                        <th className="thReport1stRow" rowSpan={3}>Job Type</th>
                        <th className="thReport1stRow" rowSpan={3}>Temp Drive</th>
                        <th className="thReport1stRow" rowSpan={3}>Use/No Use</th>
                        <th className="thReport1stRow" rowSpan={3}>Cal Time IN</th>
                        <th className="thReport1stRow" rowSpan={3}>Cal Time OUT</th>
                        <th className="thReport1stRow" colSpan={5}> Working Day</th>
                        <th className="thReport1stRow" colSpan={4}>Holiday</th>
                        <th className="thReport1stRow" rowSpan={3}>Total OT</th>
                        <th className="thReport1stRow" rowSpan={3}>Taxi</th>
                        <th className="thReport1stRow border-r" rowSpan={3}>Lunch</th>
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
                        <th className="thReport3rdRow">00:01 <p>-</p> 7:30</th>
                        <th className="thReport3rdRow">07:31 <p>-</p> 16:30</th>
                        <th className="thReport3rdRow">16:31 <p>-</p> 22:00</th>
                        <th className="thReport3rdRow">22:01 <p>-</p> 00:00</th>
                        <th className="thReport3rdRow">00:01 <p>-</p> 07:30</th>
                        <th className="thReport3rdRow">07:31 <p>-</p> 16:30</th>
                        <th className="thReport3rdRow">16:31 <p>-</p> 00:00</th>
                    </tr>
                </thead>
                <tbody>
                    {reportData.ReportList?.map((rep) => (
                        <tr key={rep.ID}>
                            <td className='tdReport'>{rep.Check_In}</td>
                            <td className='tdReport'>{rep.Check_Out}</td>
                            <td className='tdReport'>{rep.Job_Type}</td>
                            <td className='tdReport'>{rep.Temp_Drive}</td>
                            <td className='tdReport'>{rep.Use_NoUse}</td>
                            <td className='tdReport'>{rep.Cal_Time_In}</td>
                            <td className='tdReport'>{rep.Cal_Time_Out}</td>
                            <td className='tdReport Work_OT1_5_Night'>{rep.Work_OT1_5_Night}</td>
                            <td className='tdReport Work_Reg'>{rep.Work_Reg}</td>
                            <td className='tdReport Work_OT1_5_Eve'>{rep.Work_OT1_5_Eve}</td>
                            <td className='tdReport Work_OT2'>{rep.Work_OT2}</td>
                            <td className='tdReport Work_Total_OT'>{rep.Work_Total_OT}</td>
                            <td className='tdReport Holi_OT3_0'>{rep.Holi_OT3_0}</td>
                            <td className='tdReport Holi_OT2_0'>{rep.Holi_OT2_0}</td>
                            <td className='tdReport Holi_OT3_0_Eve'>{rep.Holi_OT3_0_Eve}</td>
                            <td className='tdReport Holi_Total_OT'>{rep.Holi_Total_OT}</td>
                            <td className='tdReport All_Total_OT'>{rep.All_Total_OT}</td>
                            <td className='tdReport Taxi'>{(rep.Taxi * 125)}</td>
                            <td className='tdReport Lunch'>{(rep.Lunch * 50)}</td>
                        </tr>
                    ))}
                </tbody>
                <tfoot className="overflow-y-auto">
                    {reportData.ReferReport ?
                        (
                            <>
                            <tr>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1" colSpan={5}>Total</td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1"colSpan={2}></td>
                                <td id="totalWork_OT1_5_Night" className="text-center font-bold text-red-500 border-r border-b border-gray-400 p-1" >{reportData.ReferReport.TotalWork_OT1_5_Night}</td>
                                <td id="totalWork_Reg" className="text-center font-bold border-r text-red-500 border-b border-gray-400 p-1">{reportData.ReferReport.TotalWork_Reg}</td>
                                <td id="totalWork_OT1_5_Eve" className="text-center font-bold border-r text-red-500 border-b border-gray-400 p-1">{reportData.ReferReport.TotalWork_OT1_5_Eve}</td>
                                <td id="totalWork_OT2" className="text-center font-bold border-r text-red-500 border-b border-gray-400 p-1">{reportData.ReferReport.TotalWork_OT2}</td>
                                <td id="totalWork_Total_OT" className="text-center font-bold border-r text-red-500 border-b border-gray-400 p-1">{reportData.ReferReport.TotalWork_Total_OT}</td>
                                <td id="totalHoli_OT3_0" className="text-center font-bold border-r text-red-500 border-b border-gray-400 p-1">{reportData.ReferReport.TotalHoli_OT3_0}</td>
                                <td id="totalHoli_OT2_0" className="text-center font-bold border-r text-red-500 border-b border-gray-400 p-1">{reportData.ReferReport.TotalHoli_OT2_0}</td>
                                <td id="totalHoli_OT3_0_Eve" className="text-center font-bold border-r text-red-500 border-b border-gray-400 p-1">{reportData.ReferReport.TotalHoli_OT3_0_Eve}</td>
                                <td id="totalHoli_Total_OT" className="text-center font-bold border-r text-red-500 border-b border-gray-400 p-1">{reportData.ReferReport.TotalHoli_Total_OT}</td>
                                <td id="totalAll_Total_OT" className="text-center font-bold border-r text-red-500 border-b border-gray-400 p-1">{reportData.ReferReport.TotalAll_Total_OT}</td>
                                <td id="totalTaxi" className="text-center font-bold border-r border-b text-red-500 border-gray-400 p-1">{reportData.ReferReport.TotalTaxi}</td>
                                <td id="totalLunch" className="text-center font-bold border-r border-b text-red-500 border-gray-400 p-1">{reportData.ReferReport.TotalLunch}</td>
                            </tr>
                            <tr>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1" colSpan={5}>Total Invoice</td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1" colSpan={6}></td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1">{reportData.ReferReport.TotalWork_Total_OT.replace(":15",":25").replace(":30",":50").replace(":45",":75")}</td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1" colSpan={3}></td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1">{reportData.ReferReport.TotalHoli_Total_OT.replace(":15",":25").replace(":30",":50").replace(":45",":75")}</td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1">{reportData.ReferReport.TotalAll_Total_OT.replace(":15",":25").replace(":30",":50").replace(":45",":75")}</td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1">{reportData.ReferReport.TotalTaxi*125}</td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1">{reportData.ReferReport.TotalLunch*50}</td>
                            </tr>
                            <tr>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1" colSpan={5}>Accumulate OT Driver</td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1" colSpan={11}></td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1">{reportData.ReferReport.TotalAll_Total_OT}</td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1" colSpan={2}></td>
                            </tr>
                            <tr>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1" colSpan={5}>Limit Time / Month</td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1" colSpan={11}></td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1">100:00</td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1" colSpan={2}></td>
                            </tr>
                            <tr>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1" colSpan={5}>Remaining OT Driver (Over)</td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1" colSpan={11}></td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1">{100-parseInt(reportData.ReferReport.TotalAll_Total_OT.split(':')[0])}:{reportData.ReferReport.TotalAll_Total_OT.split(':')[1]}</td>
                                <td className="text-center font-bold border-r border-b border-gray-400 p-1" colSpan={2}></td>
                            </tr>
                    </>
                    ) : null
                }
                </tfoot>
            </table>
            </div>
        </>
    )

}
