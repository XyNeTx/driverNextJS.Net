
export default function ReportInOutOutSource() {
    return (
        <div className="p-2 ">
            <h3 className="font-bold text-2xl">ค้นหาข้อมูลคนขับรถ</h3>

            <div className="flex-row-reverse mt-4 mb-4 flex">
                <button className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                    ค้นหา
                </button>
                <select title="เลือกคนขับรถ" className="border border-gray-300 rounded-md p-2 mr-2">
                    <option> -- เลือกคนขับรถ -- </option>
                </select>
                <select title="เลือกเดือน" className="border border-gray-300 rounded-md p-2 mr-2">
                    <option> -- เลือกเดือน -- </option>
                </select>
                <select title="เลือกปี" className="border border-gray-300 rounded-md p-2 mr-2">
                    <option> -- เลือกปี -- </option>
                </select>
            </div>

            <div className="block mt-4 mb-4">
                ตารางแสดงข้อมูลคนขับรถ
                <div className="overflow-x-hidden">
                    <table className="table-auto border-collapse border border-gray-400 mt-4 w-full">
                        <thead >
                            <tr className="border border-gray-400">
                                <th className="border border-gray-400 text-center p-2" rowSpan={3}>CheckIN</th>
                                <th className="border border-gray-400 text-center p-2" rowSpan={3}>CheckOut</th>
                                <th className="border border-gray-400 text-center p-2" rowSpan={3}>Job Type</th>
                                <th className="border border-gray-400 text-center p-2" rowSpan={3}>Temp Drive</th>
                                <th className="border border-gray-400 text-center p-2" rowSpan={3}>Use/No Use</th>
                                <th className="border border-gray-400 text-center p-2" rowSpan={3}>Cal Time IN</th>
                                <th className="border border-gray-400 text-center p-2" rowSpan={3}>Cal Time OUT</th>
                                <th className="border border-gray-400 text-center p-2" colSpan={5}>Working Day</th>
                                <th className="border border-gray-400 text-center p-2" colSpan={4}>Holiday</th>
                                <th className="border border-gray-400 text-center p-2" rowSpan={3}>Total OT</th>
                                <th className="border border-gray-400 text-center p-2" rowSpan={3}>Taxi</th>
                                <th className="border border-gray-400 text-center p-2" rowSpan={3}>Lunch</th>
                            </tr>
                            <tr>
                                <th className="border-r text-center p-2 " >OT1.5</th>
                                <th className="border-r text-center p-2 " >Regular Time</th>
                                <th className="border-r text-center p-2 " >OT1.5</th>
                                <th className="border-r text-center p-2 " >OT2.0</th>
                                <th className="border-r text-center p-2 border border-gray-400" rowSpan={2} >Total OT</th>
                                <th className="border-r text-center p-2 " >OT3.0</th>
                                <th className="border-r text-center p-2 " >OT2.0</th>
                                <th className="border-r text-center p-2 " >OT3.0</th>
                                <th className="border-r text-center p-2 border border-gray-400" rowSpan={2} >Total OT</th>
                            </tr>
                            <tr>
                                <th className="border-b border-r border-gray-400 text-center p-2" >00:01 - 7:29</th>
                                <th className="border-b border-r border-gray-400 text-center p-2" >07:30 - 16:30</th>
                                <th className="border-b border-r border-gray-400 text-center p-2" >16:31 - 22:00</th>
                                <th className="border-b border-r border-gray-400 text-center p-2" >22:01 - 00:00</th>
                                <th className="border-b border-r border-gray-400 text-center p-2" >00:01 - 07:29</th>
                                <th className="border-b border-r border-gray-400 text-center p-2" >07:30 - 16:30</th>
                                <th className="border-b border-r border-gray-400 text-center p-2" >16:31 - 00:00</th>
                            </tr>
                        </thead>
                    </table>
                </div>

            </div>

        </div>
    )
};