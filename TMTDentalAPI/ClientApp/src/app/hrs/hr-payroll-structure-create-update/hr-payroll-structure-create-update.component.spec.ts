import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrPayrollStructureCreateUpdateComponent } from './hr-payroll-structure-create-update.component';

describe('HrPayrollStructureCreateUpdateComponent', () => {
  let component: HrPayrollStructureCreateUpdateComponent;
  let fixture: ComponentFixture<HrPayrollStructureCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrPayrollStructureCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrPayrollStructureCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
