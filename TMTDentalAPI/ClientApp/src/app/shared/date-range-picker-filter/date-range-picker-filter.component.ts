import { Component, ElementRef, EventEmitter, Input, OnInit, Output, Renderer2, ViewChild } from '@angular/core';
import * as moment from 'moment';
import { DaterangepickerDirective } from './config/daterangepicker.directive';
import { DaterangepickerComponent } from 'ng2-daterangepicker';
@Component({
  selector: 'app-date-range-picker-filter',
  templateUrl: './date-range-picker-filter.component.html',
  styleUrls: ['./date-range-picker-filter.component.css'],
})
export class DateRangePickerFilterComponent implements OnInit {

  @Input() startDate: any;
  @Input() endDate: any;
  @Output() searchChange = new EventEmitter<any>();
  @Input() opens: string = 'auto';
  @Input() drops: string = 'auto';
  @Input() title: string = 'Thời gian';
  @Input() showClearButton: boolean = true;
  @Input() parentEl = "body";

  @Input() showDropdowns = true;
  @Input() ranges: any = {
    'Hôm nay': [moment(new Date()), moment(new Date())],
    'Hôm qua': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
    '7 ngày qua': [moment().subtract(6, 'days'), moment()],
    '30 ngày qua': [moment().subtract(29, 'days'), moment()],
    'Tháng này': [moment().startOf('month'), moment().endOf('month')],
    'Tháng trước': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
  }


  public options: any = {
    locale: {
      format: 'DD/MM/YYYY',
      customRangeLabel: "Chọn ngày",
      applyLabel: "Áp dụng",
      cancelLabel: "Đóng",
      monthNames: [
        "Tháng 1",
        "Tháng 2",
        "Tháng 3",
        "Tháng 4",
        "Tháng 5",
        "Tháng 6",
        "Tháng 7",
        "Tháng 8",
        "Tháng 9",
        "Tháng 10",
        "Tháng 11",
        "Tháng 12"
      ]
    },
    cancelClass: " btn-light",
    showDropdowns: true,
    alwaysShowCalendars: true,
    ranges: this.ranges,
    // startDate: undefined,
    // endDate: undefined,
    autoUpdateInput: true,
    opens: this.opens,
    drops: this.drops,

  };

  oldStartDate: any;
  oldEndDate: any;
  @ViewChild(DaterangepickerDirective) inputDr: DaterangepickerDirective;
  @ViewChild(DaterangepickerComponent)
  private picker: DaterangepickerComponent;
  constructor(
    private elm: ElementRef,
    private renderer: Renderer2,
  ) {

  }

  ngOnInit() {
        this.startDate = this.startDate ? moment(this.startDate): null;
        this.endDate = this.endDate ? moment(this.endDate): null;

    if (this.opens) {
      this.options.opens = this.opens;
    }
    if (this.drops) {
      this.options.drops = this.drops;
    }
    if(this.parentEl){
      this.options.parentEl = this.parentEl;
    }
  }

  ngAfterViewInit() {


    var daterangepickerEl = (this.renderer as any).engine.bodyNode.querySelector(".daterangepicker");
    var btns = daterangepickerEl.querySelector('.drp-buttons') as HTMLElement;
    var applyBtn = daterangepickerEl.querySelector('.drp-buttons .applyBtn')

    btns.insertBefore(applyBtn, btns.firstChild.nextSibling);
    if (this.showClearButton) {
      var clearEl = this.renderer.createElement(`button`);
      clearEl.innerHTML = "Hủy";
      clearEl.classList.add("btn", "btn-sm", "btn-secondary", "clearBtn");
      var cancelBtn = daterangepickerEl.querySelector('.cancelBtn');
      this.renderer.appendChild(daterangepickerEl, clearEl);
      this.renderer.listen(clearEl, 'click', (event) => {
        this.clear()
        cancelBtn.click();
      });
      applyBtn.after(clearEl);
    }

    if (this.startDate || this.endDate) {
      this.picker.datePicker.setStartDate(this.startDate);
      this.picker.datePicker.setEndDate(this.endDate);
    } else {
      // this.picker.datePicker.startDate= null;
      // this.picker.datePicker.endDate= null;
    }
  }

  public selectedDate(value: any) {

    // use passed valuable to update state
      this.startDate= value.start,
      this.endDate= value.end
    this.onApply();
  }

  clear() {
    this.startDate = null;
    this.endDate = null;
    this.onApply();
  }

  open() {
    // this.inputDr.open();
  }

  onShowDateRangePicker(e) {
    this.oldStartDate = this.startDate;
    this.oldEndDate = this.endDate;
  }

  onApply() {
    if ((this.startDate == null && this.oldStartDate != null) ||!this.startDate.isSame(this.oldStartDate) || !this.endDate.isSame(this.oldEndDate) )
    {
      var value = { dateFrom: this.startDate ? this.startDate.toDate() : null, dateTo: this.endDate ? this.endDate.toDate() : null };
      this.searchChange.emit(value);
    }
  }
}