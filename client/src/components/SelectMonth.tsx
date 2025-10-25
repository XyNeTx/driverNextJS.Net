const arrMonth = [
    { value: '01', label: 'เดือน : January' },
    { value: '02', label: 'เดือน : February' },
    { value: '03', label: 'เดือน : March' },
    { value: '04', label: 'เดือน : April' },
    { value: '05', label: 'เดือน : May' },
    { value: '06', label: 'เดือน : June' },
    { value: '07', label: 'เดือน : July' },
    { value: '08', label: 'เดือน : August' },
    { value: '09', label: 'เดือน : September' },
    { value: '10', label: 'เดือน : October' },
    { value: '11', label: 'เดือน : November' },
    { value: '12', label: 'เดือน : December' },
]


export default async function SelectMonth() {
    return (
        <>
            <select title="เลือกเดือน" className="border border-gray-300 rounded-md p-2 mr-2">
                <option> -- เลือกเดือน -- </option>
                {arrMonth.map((month) => (
                    <option key={month.value} value={month.value}>{month.label}</option>
                ))}
            </select>
        </>
    )
}
