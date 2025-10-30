const currentYear = new Date().getFullYear();

const arrYear = [
    { value : (currentYear-3).toString(), label : "ปี : "+ (currentYear-3).toString() }
];

for(let i = currentYear - 2;i <= currentYear;i++){
    arrYear.push({ value : i.toString(), label : "ปี : " + i.toString()})
}

interface SelectProps {
  value: string;
  onChange: (val: string) => void;
}


export default function SelectYear({value,onChange} : SelectProps) {
    return (
        <>
            <select
                value={value}
                onChange={(e)=> onChange(e.target.value)}
                title="เลือกปี" className="border border-gray-300 rounded-md p-2 mr-2">
                <option> -- เลือกปี -- </option>
                {arrYear.map((year) => (
                    <option key={year.value} value={year.value}>{year.label}</option>
                ))}
            </select>
        </>
    )
}