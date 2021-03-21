import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeAdminUpdateComponent } from './employee-admin-update.component';

describe('EmployeeAdminUpdateComponent', () => {
  let component: EmployeeAdminUpdateComponent;
  let fixture: ComponentFixture<EmployeeAdminUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EmployeeAdminUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeAdminUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
