import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrSalaryConfigCreateUpdateComponent } from './hr-salary-config-create-update.component';

describe('HrSalaryConfigCreateUpdateComponent', () => {
  let component: HrSalaryConfigCreateUpdateComponent;
  let fixture: ComponentFixture<HrSalaryConfigCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrSalaryConfigCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrSalaryConfigCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
