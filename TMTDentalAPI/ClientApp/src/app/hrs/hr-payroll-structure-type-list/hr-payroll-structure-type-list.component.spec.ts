import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrPayrollStructureTypeListComponent } from './hr-payroll-structure-type-list.component';

describe('HrPayrollStructureTypeListComponent', () => {
  let component: HrPayrollStructureTypeListComponent;
  let fixture: ComponentFixture<HrPayrollStructureTypeListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrPayrollStructureTypeListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrPayrollStructureTypeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
