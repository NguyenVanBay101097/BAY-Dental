import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareQuickreplyDialogComponent } from './tcare-quickreply-dialog.component';

describe('TcareQuickreplyDialogComponent', () => {
  let component: TcareQuickreplyDialogComponent;
  let fixture: ComponentFixture<TcareQuickreplyDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareQuickreplyDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareQuickreplyDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
