import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareMessageTemplateContentComponent } from './tcare-message-template-content.component';

describe('TcareMessageTemplateContentComponent', () => {
  let component: TcareMessageTemplateContentComponent;
  let fixture: ComponentFixture<TcareMessageTemplateContentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareMessageTemplateContentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareMessageTemplateContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
