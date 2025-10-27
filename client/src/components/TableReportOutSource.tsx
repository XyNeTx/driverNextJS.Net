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

export default function TableReportOutSource({reportData}: {reportData : Driver_OutSource[]}){
    return (
        <>
            <div className="">
                <table className="table-auto border-collapse border border-gray-400 mt-4 w-full overflow-y-auto overflow-x-auto">
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
                            <th className="thReport3rdRow">16:31 - 22:00</th>
                            <th className="thReport3rdRow">22:01 - 00:00</th>
                            <th className="thReport3rdRow">00:01 - 07:30</th>
                            <th className="thReport3rdRow">07:31 - 16:30</th>
                            <th className="thReport3rdRow">16:31 - 00:00</th>
                        </tr>
                    </thead>
                    <tbody>
                        {reportData.map((rep) => (
                            <tr key={rep.ID}>
                                <td className='tdReport'>{rep.Check_In}</td>
                                <td className='tdReport'>{rep.Check_Out}</td>
                                <td className='tdReport'>{rep.Job_Type}</td>
                                <td className='tdReport'>{rep.Temp_Drive}</td>
                                <td className='tdReport'>{rep.Use_NoUse}</td>
                                <td className='tdReport'>{rep.Cal_Time_In}</td>
                                <td className='tdReport'>{rep.Cal_Time_Out}</td>
                                <td className='tdReport'>{rep.Work_OT1_5_Night}</td>
                                <td className='tdReport'>{rep.Work_Reg}</td>
                                <td className='tdReport'>{rep.Work_OT1_5_Eve}</td>
                                <td className='tdReport'>{rep.Work_OT2}</td>
                                <td className='tdReport'>{rep.Work_Total_OT}</td>
                                <td className='tdReport'>{rep.Holi_OT3_0}</td>
                                <td className='tdReport'>{rep.Holi_OT2_0}</td>
                                <td className='tdReport'>{rep.Holi_OT3_0_Eve}</td>
                                <td className='tdReport'>{rep.Holi_Total_OT}</td>
                                <td className='tdReport'>{rep.All_Total_OT}</td>
                                <td className='tdReport'>{(rep.Taxi * 125)}</td>
                                <td className='tdReport'>{(rep.Lunch * 50)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </>
    )

}
