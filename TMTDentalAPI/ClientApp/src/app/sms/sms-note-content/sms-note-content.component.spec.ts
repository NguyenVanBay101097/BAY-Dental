import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsNoteContentComponent } from './sms-note-content.component';

describe('SmsNoteContentComponent', () => {
  let component: SmsNoteContentComponent;
  let fixture: ComponentFixture<SmsNoteContentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsNoteContentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsNoteContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
