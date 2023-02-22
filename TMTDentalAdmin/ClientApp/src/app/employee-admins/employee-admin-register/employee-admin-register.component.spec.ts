import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeAdminRegisterComponent } from './employee-admin-register.component';

describe('EmployeeAdminRegisterComponent', () => {
  let component: EmployeeAdminRegisterComponent;
  let fixture: ComponentFixture<EmployeeAdminRegisterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EmployeeAdminRegisterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeAdminRegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
