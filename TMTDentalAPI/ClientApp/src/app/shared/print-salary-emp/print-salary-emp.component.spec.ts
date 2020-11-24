import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintSalaryEmpComponent } from './print-salary-emp.component';

describe('PrintSalaryEmpComponent', () => {
  let component: PrintSalaryEmpComponent;
  let fixture: ComponentFixture<PrintSalaryEmpComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintSalaryEmpComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintSalaryEmpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
