import { EmployeeBasic } from './../../employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { debounceTime, tap, switchMap, map } from 'rxjs/operators';
import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { HrPaysliprunService, PaySlipRunConfirmViewModel } from '../hr-paysliprun.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { HrPayrollStructureService, HrPayrollStructureDisplay, HrPayrollStructurePaged } from '../hr-payroll-structure.service';
import { SelectEmployeeDialogComponent } from 'src/app/shared/select-employee-dialog/select-employee-dialog.component';
import { EmployeePaged } from 'src/app/employees/employee';
import { Subject } from 'rxjs';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-hr-payslip-run-confirm-dialog',
  templateUrl: './hr-payslip-run-confirm-dialog.component.html',
  styleUrls: ['./hr-payslip-run-confirm-dialog.component.css']
})
export class HrPayslipRunConfirmDialogComponent implements OnInit {
  myForm: FormGroup;
  lineForm: FormGroup;
  @Input() public id: string;
  submitted = false;
  filterStructures: HrPayrollStructureDisplay[];
  skip = 0;
  pageSize = 20;
  search: string;
  searchUpdate = new Subject<string>();
  liness : EmployeeBasic[];
  isDoctor: boolean;
  isAssistant: boolean;
  isOther: boolean;


  @ViewChild('structureCbx', { static: true }) structureCbx: ComboBoxComponent;
 
  title: string;
  constructor(private fb: FormBuilder, private hrPaysliprunService: HrPaysliprunService, private intlService: IntlService,
    private hrPayrollStructureService : HrPayrollStructureService,
    private employeeService : EmployeeService,
    public activeModal: NgbActiveModal,
    private activeroute: ActivatedRoute,
    private modalService: NgbModal,
    private errorService: AppSharedShowErrorService) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      structure: [null , Validators.required],
      lines: null,
    });
    this.loadFilteredStructures();

    this.structureCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.structureCbx.loading = true)),
      switchMap(value => this.searchFilteredStructures(value))
    ).subscribe(result => {
      this.filterStructures = result.items;
      this.structureCbx.loading = false;
    });

  }

  openEmployees(){
    let modalRef = this.modalService.open(SelectEmployeeDialogComponent, { size: "lg", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Chọn nhân viên";
    modalRef.result.then(
      rs => {             
        this.addEmployee(rs);
      },
      () => { }
    );
  }

  addEmployee(idss){
    if(idss){
        var val = new EmployeePaged();
        val.ids = idss;
        this.employeeService.getSearchRead(val).subscribe(
          res=> {            
            this.liness = res.items;
          }
        );      
      }
  }

  get lines() {
    var val = this.myForm.value;
    val.lines =  this.liness
    return val.lines;
  }

  loadFilteredStructures(){
    this.searchFilteredStructures().subscribe(
      res=> {
        this.filterStructures = res.items;
      }
    )
  }

  actionConfirm(){
    debugger
    var val = new PaySlipRunConfirmViewModel();
    var myform = this.myForm.value;
    val.payslipRunId = this.id;
    val.structureId = myform.structure ? myform.structure.id : null;
    val.employees = myform.lines ? myform.lines : null;
    this.hrPaysliprunService.actionConfirm(val).subscribe(
      () => { 
        this.activeModal.close(); 
        
      });
    }
  


  searchFilteredStructures(filter?: string) {
    var val = new HrPayrollStructurePaged();
    val.filter = filter? filter : '';
    return this.hrPayrollStructureService.getPaged(val);
  }

  deleteLine(index: number) {
    this.lines.removeAt(index);      
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  get f() {
    return this.myForm.controls;
  }

}
