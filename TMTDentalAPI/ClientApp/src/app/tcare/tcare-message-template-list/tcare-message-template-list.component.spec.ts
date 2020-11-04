import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareMessageTemplateListComponent } from './tcare-message-template-list.component';

describe('TcareMessageTemplateListComponent', () => {
  let component: TcareMessageTemplateListComponent;
  let fixture: ComponentFixture<TcareMessageTemplateListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareMessageTemplateListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareMessageTemplateListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
