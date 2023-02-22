import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-print-salary-emp',
  templateUrl: './print-salary-emp.component.html',
  styleUrls: ['./print-salary-emp.component.css'],
  host: {
    'class': 'h-100 w-100',
    'id': 'salary-print'
  }
})
export class PrintSalaryEmpComponent implements OnInit {

  public salaryDetails: any[] = [
    {
      no: 1, lable: 'Lương cơ bản', content: 6740741
    },
    {
      no: 2, lable: 'Lương tăng ca', content: 291667
    },
    {
      no: 3, lable: 'Lương làm thêm', content: 622000
    },
    {
      no: 4, lable: 'Phụ cấp xác định', content: 1000000
    },
    {
      no: 5, lable: 'Phụ cấp khác', content: 0
    },
    {
      no: 6, lable: 'Thưởng', content: 1000000
    },
    {
      no: 7, lable: 'Phục cấp lễ tết', content: 100000
    },
    {
      lable: 'Tổng lương', content: 9754630
    },
    {
      no: 8, lable: 'Hoa hồng', content: 5000000
    },
    {
      no: 9, lable: 'Phạt', content: 0
    },
    {
      no: 10, lable: 'Tạm ứng', content: 2000000
    },
    {
      lable: 'Thực lĩnh', content: 12749032
    }
  ]

  constructor() { }

  ngOnInit() {
  }

}
