import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToothDiagnosisCreateUpdateDialogComponent } from './tooth-diagnosis-create-update-dialog.component';

describe('ToothDiagnosisCreateUpdateDialogComponent', () => {
  let component: ToothDiagnosisCreateUpdateDialogComponent;
  let fixture: ComponentFixture<ToothDiagnosisCreateUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToothDiagnosisCreateUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToothDiagnosisCreateUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
