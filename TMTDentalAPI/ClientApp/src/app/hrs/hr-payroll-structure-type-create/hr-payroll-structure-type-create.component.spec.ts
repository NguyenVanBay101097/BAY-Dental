import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrPayrollStructureTypeCreateComponent } from './hr-payroll-structure-type-create.component';

describe('HrPayrollStructureTypeCreateComponent', () => {
  let component: HrPayrollStructureTypeCreateComponent;
  let fixture: ComponentFixture<HrPayrollStructureTypeCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrPayrollStructureTypeCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrPayrollStructureTypeCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
