import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TreatmentProcessServiceListComponent } from './treatment-process-service-list.component';

describe('TreatmentProcessServiceListComponent', () => {
  let component: TreatmentProcessServiceListComponent;
  let fixture: ComponentFixture<TreatmentProcessServiceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TreatmentProcessServiceListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TreatmentProcessServiceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
