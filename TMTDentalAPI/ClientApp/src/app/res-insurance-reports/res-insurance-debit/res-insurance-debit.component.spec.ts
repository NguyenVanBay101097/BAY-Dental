import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResInsuranceDebitComponent } from './res-insurance-debit.component';

describe('ResInsuranceDebitComponent', () => {
  let component: ResInsuranceDebitComponent;
  let fixture: ComponentFixture<ResInsuranceDebitComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResInsuranceDebitComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResInsuranceDebitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
