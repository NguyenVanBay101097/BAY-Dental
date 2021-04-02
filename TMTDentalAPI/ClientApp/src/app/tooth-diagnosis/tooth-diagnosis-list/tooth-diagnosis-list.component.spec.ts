import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToothDiagnosisListComponent } from './tooth-diagnosis-list.component';

describe('ToothDiagnosisListComponent', () => {
  let component: ToothDiagnosisListComponent;
  let fixture: ComponentFixture<ToothDiagnosisListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToothDiagnosisListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToothDiagnosisListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
