import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrPayrollStructureListComponent } from './hr-payroll-structure-list.component';

describe('HrPayrollStructureListComponent', () => {
  let component: HrPayrollStructureListComponent;
  let fixture: ComponentFixture<HrPayrollStructureListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrPayrollStructureListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrPayrollStructureListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
