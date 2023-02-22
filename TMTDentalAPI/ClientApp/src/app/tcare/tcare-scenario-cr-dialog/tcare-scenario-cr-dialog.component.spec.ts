import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareScenarioCrDialogComponent } from './tcare-scenario-cr-dialog.component';

describe('TcareScenarioCrDialogComponent', () => {
  let component: TcareScenarioCrDialogComponent;
  let fixture: ComponentFixture<TcareScenarioCrDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareScenarioCrDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareScenarioCrDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
